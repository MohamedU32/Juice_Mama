using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class CustomersManager : MonoBehaviour
{
    public static CustomersManager Instance { get; private set; }

    [Header("Customer Spawning")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Vector2 spawnIntervalRange = new Vector2(2f, 5f);
    [SerializeField] private Transform exitPoint;

    [Header("Customer Count Tracking")]
    private int currentCustomerCount = 0;

    const string SellingStandTag = "SellingStand";
    float spawnTimer;
    int spawnedCount;
    readonly Queue<NavMeshAgent> waitingAgents = new Queue<NavMeshAgent>();
    StandQueueController[] stands;
    Coroutine retryRoutine;
    int maxCustomers = 1;
    int totalServed;
    int nextAdjustAt = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        RefreshStands();
        InvokeRepeating(nameof(RefreshStands), 5f, 5f);
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

        // Increment customer count when spawned
        UpdateCustomersCount();

        if (!AssignToStand(agent))
        {
            waitingAgents.Enqueue(agent);
        }

        if (retryRoutine == null)
        {
            retryRoutine = StartCoroutine(RetryAssignLoop());
        }
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
            if (s && !s.IsFull && s.Enqueue(agent))
            {
                var customerController = agent.GetComponent<CustomerAgentController>();
                if (customerController != null)
                {
                    customerController.SetStand(s);
                    var thought = agent.GetComponentInChildren<ThoughtBubble>();
                    if (thought != null)
                    {
                        thought.ShowThought(0, 5f, s.GetJuiceData().icon);
                    }
                }
                return true;
            }
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
        totalServed++;
        if (totalServed >= nextAdjustAt)
        {
            AdjustMaxCustomers();
            nextAdjustAt = totalServed + Random.Range(3, 6);
        }
        OnCustomerLeft(agent);
    }

    public void OnCustomerLeft(NavMeshAgent agent)
    {
        if (!agent || !exitPoint) return;

        agent.SetDestination(exitPoint.position);
        Destroy(agent.gameObject, 10f);
        spawnedCount = Mathf.Max(0, spawnedCount - 1);
        UpdateCustomersCount();
    }

    public void OnCustomerLeftUnserved(NavMeshAgent agent)
    {
        if (!agent || !exitPoint) return;

        agent.SetDestination(exitPoint.position);
        Destroy(agent.gameObject, 10f);
        spawnedCount = Mathf.Max(0, spawnedCount - 1);
        Debug.Log("Customer left unserved due to impatience!");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCustomerLeftNotification();
        }
        UpdateCustomersCount();
    }

    void AdjustMaxCustomers()
    {
        var standCount = stands != null ? stands.Length : 0;
        if (standCount <= 0)
        {
            maxCustomers = 1;
            return;
        }
        var upper = Mathf.Max(1, standCount * 2);
        var delta = Random.Range(-1, 2);
        var next = Mathf.Clamp(maxCustomers + delta, 1, upper);
        maxCustomers = next;
    }

    public int CurrentCustomerCount => currentCustomerCount;

    public void UpdateCustomersCount()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCustomerCount(spawnedCount);
        }
    }
}