using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    public Transform targetPoint; // Целевая точка
    private NavMeshAgent agent; // Ссылка на нашего агента

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Получаем компонент NavMeshAgent
        agent.SetDestination(targetPoint.position); // Устанавливаем цель для агента
    }

    void Update() { }
}
