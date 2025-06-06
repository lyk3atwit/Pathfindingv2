using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSetup : MonoBehaviour
{
    [Header("Agent & Waypoints")]
    public GameObject agent;                     // Manually placed agent in the scene
    public Transform[] waypointObjects;          // Manually placed waypoints in scene

    [Header("Optional")]
    public NavMeshSurface navMeshSurface;        // Optional if you use runtime navmesh baking

    void Start()
    {
        // 1. Build navmesh at runtime if needed
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh rebuilt at runtime.");
        }

        // 2. Assign waypoints to agent’s patrol script
        if (agent != null && waypointObjects != null && waypointObjects.Length > 0)
        {
            NavMeshPatrolAgent patrolScript = agent.GetComponent<NavMeshPatrolAgent>();
            if (patrolScript != null)
            {
                patrolScript.waypoints = waypointObjects;
                Debug.Log("Waypoints assigned to agent.");
            }
            else
            {
                Debug.LogWarning("PatrolAgent script not found on agent.");
            }
        }
        else
        {
            Debug.LogWarning("Agent or waypoint objects not assigned.");
        }
    }
}
