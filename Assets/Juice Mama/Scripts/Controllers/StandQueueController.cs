using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;
using TMPro;

public class StandQueueController : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> points;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private StorageController fridgeStorage;
    [SerializeField] private JuiceData juiceData;
    [SerializeField] private TextMeshPro juiceCountText;

    [SerializeField]
    private int maxQueueLength = 4;
    private readonly List<NavMeshAgent> q = new List<NavMeshAgent>();
    private bool isServing = false;
    private int totalJuiceCount = 0;

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

    private void Start()
    {
        if (fridgeStorage == null)
        {
            Debug.LogError("Fridge StorageController is not assigned in StandQueueController.");
        }
        if (juiceData == null)
        {
            Debug.LogError("JuiceData is not assigned in StandQueueController.");
        }
        UpdateJuiceCount(juiceData);
    }

    public JuiceData GetJuiceData()
    {
        return juiceData;
    }

    public void UpdateJuiceCount(JuiceData data)
    {
        if (data == null) return;
        totalJuiceCount = fridgeStorage.GetCount(data);
        if (juiceCountText != null)
        {
            juiceCountText.text = totalJuiceCount.ToString();
        }
    }

    public void ServeNextCustomer()
    {
        if (isServing) return;
        if (fridgeStorage.GetCount(juiceData) <= 0 || q.Count == 0)
        {
            AudioManager.Instance.PlaySound(AudioNames.FAILED_COLLECTION, 1.0f);
            return;
        }
        if (!q[0].gameObject.GetComponent<CustomerAgentController>().isWaiting)
        {
            AudioManager.Instance.PlaySound(AudioNames.FAILED_COLLECTION, 1.0f);
            return;
        }
        StartCoroutine(ServiceRoutine());
    }

    private IEnumerator ServiceRoutine()
    {
        isServing = true;
        yield return new WaitForSeconds(0.8f);
        fridgeStorage.Remove(juiceData, 1);
        playerData.money += juiceData.price;
        UIManager.Instance.UpdateMoney();
        UpdateJuiceCount(juiceData);
        AudioManager.Instance.PlaySound(AudioNames.JUICE_SOLD, 1.0f);
        CustomersManager.Instance.OnCustomerServed(DequeueFront());
        isServing = false;
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

    public bool IsInFront(NavMeshAgent agent)
    {
        if (agent == null || q.Count == 0) return false;
        if (q[0] == agent) return true;
        return false;
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

    void OnFridgeLoaded()
    {
        UpdateJuiceCount(juiceData);
    }

    void OnEnable()
    {
        GameEvents.OnFridgeLoaded += OnFridgeLoaded;
    }

    void OnDisable()
    {
        GameEvents.OnFridgeLoaded -= OnFridgeLoaded;
    }
}
