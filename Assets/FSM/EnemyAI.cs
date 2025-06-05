using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 5f;

    private NavMeshAgent agent;
    private IState currentState;

    public PatrolState patrolState;
    public ChaseState chaseState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        patrolState = new PatrolState(this, agent, patrolPoints);
        chaseState = new ChaseState(this, agent, player, chaseRange);

        TransitionToState(patrolState);
    }

    void Update()
    {
        currentState.Execute();
    }

    public void TransitionToState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }
}
