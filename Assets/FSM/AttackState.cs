using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AttackState : IState
{
    private EnemyAI ai;
    private NavMeshAgent agent;
    private Transform player;
    private Text stateText;

    public AttackState(EnemyAI ai, NavMeshAgent agent, Transform player, Text stateText)
    {
        this.ai = ai;
        this.agent = agent;
        this.player = player;
        this.stateText = stateText;
    }

    public void Enter()
    {
        agent.isStopped = true;
        ai.GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("Attacking!");
    }

    public void Execute()
    {
        float dist = Vector3.Distance(ai.transform.position, player.position);
        
        if (dist > ai.attackRange * 1.2f)
        {
            ai.TransitionToState(ai.chaseState);
        }
        else if (dist > ai.chaseRange)
        {
            ai.TransitionToState(ai.patrolState);
        }
    }

    public void Exit()
    {
        agent.isStopped = false;
    }
}