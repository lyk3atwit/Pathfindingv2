using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPatrolAgent : MonoBehaviour
{
    public Transform[] waypoints;
    private int index = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            index = (index + 1) % waypoints.Length;
            agent.SetDestination(waypoints[index].position);
        }
    }
}
