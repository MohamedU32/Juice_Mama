using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    protected NavMeshAgent agent;
    public Animator animator;
    [SerializeField] float interactRange = 1.2f;

    [SerializeField] bool useInteractRangeAsStopDistance = false;

    public bool isWaiting = false;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = useInteractRangeAsStopDistance ? interactRange : agent.stoppingDistance;
    }

    protected void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        isWaiting = (agent.remainingDistance <= agent.stoppingDistance) && !agent.pathPending;
    }
    protected void MoveTo(Vector3 p)
    {
        if (agent != null)
        {
            agent.SetDestination(p);
        }
    }

    protected bool Arrived()
    {
        if (agent == null) return false;
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }
}
