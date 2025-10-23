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
}
