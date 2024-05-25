using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaInfo : MonoBehaviour
{
    [Serializable]
    public class Neighbor
    {
        public AreaInfo area;
        public Transform transitionPoint;
    }

    [SerializeField]
    public Neighbor[] neighbors;

    public static List<AreaInfo> areas = new List<AreaInfo>();

    void Awake()
    {
        areas.Add(this);
    }

    // Method to generate a path between two areas
    public static List<AreaInfo> FindPathBetweenAreas(AreaInfo start, AreaInfo end)
    {
        // Create a queue of areas to visit
        Queue<AreaInfo> queue = new Queue<AreaInfo>();
        queue.Enqueue(start);

        // Create a dictionary to store the parent of each area
        Dictionary<AreaInfo, AreaInfo> parent = new Dictionary<AreaInfo, AreaInfo>();
        parent[start] = null;

        // While there are areas to visit
        while (queue.Count > 0)
        {
            // Get the current area
            AreaInfo current = queue.Dequeue();

            // If we have reached the end area, reconstruct the path
            if (current == end)
            {
                List<AreaInfo> path = new List<AreaInfo>();
                while (current != null)
                {
                    path.Add(current);
                    current = parent[current];
                }
                path.Reverse();
                return path;
            }

            // Visit the neighbors of the current area
            foreach (Neighbor neighbor in current.neighbors)
            {
                if (!parent.ContainsKey(neighbor.area))
                {
                    queue.Enqueue(neighbor.area);
                    parent[neighbor.area] = current;
                }
            }
        }

        // If no path is found, return an empty list
        return new List<AreaInfo>();
    }
}
