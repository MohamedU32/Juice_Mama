using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class StandQueueController : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> points;

    [SerializeField]
    private int maxQueueLength = 4;
    private readonly List<NavMeshAgent> q = new List<NavMeshAgent>();

    private void Awake()
    {
        points = new List<Vector3>();
        Vector3 startPoint = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = transform.right * 1.5f;
        for (int i = 0; i < maxQueueLength; i++)
        {
            points.Add(startPoint + direction * i + transform.right * 1.5f);
        }
    }

    public void ServeNextCustomer()
    {
        CustomersManager.Instance.OnCustomerServed(DequeueFront());
    }

    public bool IsFull => q.Count >= maxQueueLength;

    public bool Enqueue(NavMeshAgent agent)
    {
        if (agent == null || points == null || points.Count == 0) return false;
        if (IsFull) return false;
        if (q.Contains(agent)) return false;
        q.Add(agent);
        UpdateTargets();
        return true;
    }

    public bool IsQueued(NavMeshAgent agent) => agent && q.Contains(agent);
    public int GetQueueIndex(NavMeshAgent agent) => agent ? q.IndexOf(agent) : -1;

    public NavMeshAgent DequeueFront()
    {
        if (q.Count == 0) return null;
        var f = q[0];
        q.RemoveAt(0);
        UpdateTargets();
        f.SetDestination(Vector3.zero);
        return f;
    }

    public void Remove(NavMeshAgent a)
    {
        if (a == null) return;
        if (q.Remove(a)) UpdateTargets();
    }

    private void UpdateTargets()
    {
        for (int i = 0; i < q.Count; i++)
        {
            if (i >= points.Count) break;
            var p = points[i];
            if (p != Vector3.zero) q[i].SetDestination(p);
        }
    }

    void LateUpdate()
    {
        if (q.Count == 0) return;
        var fwd = -transform.right;
        for (int i = 0; i < q.Count; i++)
        {
            var a = q[i];
            if (!a) continue;
            if (!a.pathPending && a.remainingDistance <= a.stoppingDistance + 0.05f)
            {
                var rot = Quaternion.LookRotation(fwd, Vector3.up);
                a.transform.rotation = Quaternion.RotateTowards(a.transform.rotation, rot, 360f * Time.deltaTime);
            }
        }
    }
}
