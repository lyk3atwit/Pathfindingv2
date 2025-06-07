using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FleeState : IState
{
    private EnemyAI ai;
    private NavMeshAgent agent;
    private Transform fleePoint;
    private Text stateText;

    public FleeState(EnemyAI ai, NavMeshAgent agent, Transform fleePoint, Text stateText)
    {
        this.ai = ai;
        this.agent = agent;
        this.fleePoint = fleePoint;
        this.stateText = stateText;
    }

    public void Enter()
    {
        ai.GetComponent<Renderer>().material.color = Color.blue;
        stateText.text = "State: Flee";
        agent.SetDestination(fleePoint.position);
    }

    public void Execute()
    {
        // Return to patrol if health recovers or reached safe distance
        if (ai.health > 50f || agent.remainingDistance < 0.5f)
        {
            ai.TransitionToState(ai.patrolState);
        }
    }

    public void Exit()
    {
        // Cleanup if needed
    }
}