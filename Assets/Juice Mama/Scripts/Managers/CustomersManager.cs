using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class CustomersManager : MonoBehaviour
{
    public static CustomersManager Instance { get; private set; }
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public int maxCustomers = 10;
    public Vector2 spawnIntervalRange = new Vector2(2f, 5f);
    [SerializeField] Transform exitPoint;
    const string SellingStandTag = "SellingStand";
    float spawnTimer;
    int spawnedCount;
    readonly Queue<NavMeshAgent> waitingAgents = new Queue<NavMeshAgent>();
    StandQueueController[] stands;
    Coroutine retryRoutine;

    void Awake()
    {
        RefreshStands();
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject);
    }

    void Update()
    {
        if (!CanSpawn()) return;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) return;
        SpawnCustomer();
        spawnTimer = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
    }

    bool CanSpawn()
    {
        return spawnedCount < maxCustomers && customerPrefab && spawnPoint && stands != null && stands.Length > 0;
    }

    void SpawnCustomer()
    {
        var go = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        var agent = go.GetComponent<NavMeshAgent>();
        spawnedCount++;
        if (!AssignToStand(agent)) waitingAgents.Enqueue(agent);
        if (retryRoutine == null) retryRoutine = StartCoroutine(RetryAssignLoop());
    }

    void RefreshStands()
    {
        var tagged = GameObject.FindGameObjectsWithTag(SellingStandTag);
        var list = new List<StandQueueController>();
        for (int i = 0; i < tagged.Length; i++)
        {
            var c = tagged[i].GetComponent<StandQueueController>();
            if (c) list.Add(c);
        }
        stands = list.ToArray();
    }

    bool AssignToStand(NavMeshAgent agent)
    {
        if (!agent) return false;
        if (stands == null || stands.Length == 0) RefreshStands();
        if (stands == null || stands.Length == 0) return false;
        int start = Random.Range(0, stands.Length);
        for (int i = 0; i < stands.Length; i++)
        {
            var s = stands[(start + i) % stands.Length];
            if (s && !s.IsFull && s.Enqueue(agent)) return true;
        }
        return false;
    }

    IEnumerator RetryAssignLoop()
    {
        var wait = new WaitForSeconds(0.75f);
        while (waitingAgents.Count > 0)
        {
            int n = waitingAgents.Count;
            for (int i = 0; i < n; i++)
            {
                var a = waitingAgents.Dequeue();
                if (!AssignToStand(a)) waitingAgents.Enqueue(a);
            }
            yield return wait;
        }
        retryRoutine = null;
    }

    public void OnCustomerServed(NavMeshAgent agent)
    {
        if (!agent || !exitPoint) return;
        agent.SetDestination(exitPoint.position);
        Destroy(agent.gameObject, 10f);
        spawnedCount = Mathf.Max(0, spawnedCount - 1);
    }
}
