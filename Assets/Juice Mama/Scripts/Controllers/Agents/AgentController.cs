using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    protected NavMeshAgent agent;
    public Animator animator;

    public bool isWaiting = false;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        isWaiting = agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }

    // Helper to move agent to a world position
    protected void MoveTo(Vector3 position)
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent == null) return;
        agent.SetDestination(position);
    }

    // Returns true if the agent has arrived at its destination (or is stopped)
    protected bool Arrived()
    {
        if (agent == null) return true;
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }
}
