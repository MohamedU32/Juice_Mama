using UnityEngine;
using UnityEngine.AI;

public class CustomerAgentController : AgentController
{
    [SerializeField]
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    void Start()
    {
        base.Start();
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        base.Update();

        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
