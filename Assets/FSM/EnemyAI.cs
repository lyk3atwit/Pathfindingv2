using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float health = 100f;
    public Text stateText;

    private NavMeshAgent agent;
    private IState currentState;

    public PatrolState patrolState;
    public ChaseState chaseState;
    public AttackState attackState;
    public FleeState fleeState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        patrolState = new PatrolState(this, agent, patrolPoints);
        chaseState = new ChaseState(this, agent, player, chaseRange);
        attackState = new AttackState(this, agent, player, stateText);
        fleeState = new FleeState(this, agent, patrolPoints[0], stateText);

        TransitionToState(patrolState);
    }

    void Update()
    {
        currentState?.Execute();
        
        // Update UI
        if (stateText != null)
        {
            stateText.text = "State: " + currentState.GetType().Name;
        }

        // Bonus: Check for low health to flee
        if (health <= 30f && currentState != fleeState)
        {
            TransitionToState(fleeState);
        }
    }

    public void TransitionToState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }
}