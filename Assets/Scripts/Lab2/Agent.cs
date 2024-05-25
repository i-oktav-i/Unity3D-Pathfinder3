using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public Transform targetPoint; // Целевая точка
    Vector3 lastTargetPointPosition;
    private NavMeshAgent agent; // Ссылка на нашего агента

    Coroutine moveCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Получаем компонент NavMeshAgent
    }

    void Update()
    {
        if (lastTargetPointPosition != targetPoint.position)
        {
            lastTargetPointPosition = targetPoint.position;
            GoToTarget();
        }
    }

    void GoToTarget()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        List<Vector3> pathNodes = GetPathNodes();

        Queue<Vector3> path = new Queue<Vector3>(pathNodes);

        moveCoroutine = StartCoroutine(MoveAlongPath(path));
    }

    // Корутина перемещения по пути
    public IEnumerator MoveAlongPath(Queue<Vector3> path)
    {
        agent.ResetPath();
        while (path.Count > 0)
        {
            Vector3 target = path.Dequeue();
            NavMeshPath navMeshPath = new();

            while (
                !(
                    agent.CalculatePath(target, navMeshPath)
                    && navMeshPath.status == NavMeshPathStatus.PathComplete
                )
            )
                yield return new WaitForFixedUpdate();

            agent.SetPath(navMeshPath);

            while (agent.remainingDistance > 0.1f)
                yield return new WaitForFixedUpdate();
        }

        moveCoroutine = null;
    }

    List<Vector3> GetPathNodes()
    {
        AreaInfo targetPointArea = AreaInfo.areas.Find(area =>
        {
            NavMeshPath path = new();
            NavMesh.CalculatePath(
                area.neighbors[0].transitionPoint.position,
                targetPoint.position,
                NavMesh.AllAreas,
                path
            );

            return path.status == NavMeshPathStatus.PathComplete;
        });

        if (targetPointArea == null)
            return new List<Vector3>();

        Component meshOwner = (Component)agent.navMeshOwner;
        AreaInfo startArea = meshOwner.GetComponent<AreaInfo>();

        List<AreaInfo> pathAreas = AreaInfo.FindPathBetweenAreas(startArea, targetPointArea);

        Queue<AreaInfo> queue = new Queue<AreaInfo>(pathAreas);

        AreaInfo lastArea = queue.Dequeue();

        List<Vector3> pathNodes = new List<Vector3>();

        while (queue.Count > 0)
        {
            AreaInfo currentArea = queue.Dequeue();

            List<AreaInfo.Neighbor> lastAreaNeighbors = new List<AreaInfo.Neighbor>(
                lastArea.neighbors
            );
            List<AreaInfo.Neighbor> currentAreaNeighbors = new List<AreaInfo.Neighbor>(
                currentArea.neighbors
            );

            Vector3 nodeToNext = lastAreaNeighbors
                .Find(neighbor => neighbor.area == currentArea)
                .transitionPoint.position;

            Vector3 nodeInCurrent = currentAreaNeighbors
                .Find(neighbor => neighbor.area == lastArea)
                .transitionPoint.position;

            pathNodes.Add(nodeToNext);
            pathNodes.Add(nodeInCurrent);

            lastArea = currentArea;
        }

        pathNodes.Add(targetPoint.position);

        return pathNodes;
    }
}
