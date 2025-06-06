using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    private EnemyAI ai;
    private NavMeshAgent agent;
    private Transform[] waypoints;
    private int index = 0;

    public PatrolState(EnemyAI ai, NavMeshAgent agent, Transform[] waypoints)
    {
        this.ai = ai;
        this.agent = agent;
        this.waypoints = waypoints;
    }

    public void Enter()
    {
        agent.SetDestination(waypoints[index].position);
    }

    public void Execute()
    {
        float distToPlayer = Vector3.Distance(ai.transform.position, ai.player.position);
        if (distToPlayer < ai.chaseRange)
        {
            ai.TransitionToState(ai.chaseState);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            index = (index + 1) % waypoints.Length;
            agent.SetDestination(waypoints[index].position);
        }
    }

    public void Exit()
    {
        // Optional: cleanup or animation
    }
}

