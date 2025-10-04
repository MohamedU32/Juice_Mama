using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    protected NavMeshAgent agent;
    public Animator animator;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }
}
