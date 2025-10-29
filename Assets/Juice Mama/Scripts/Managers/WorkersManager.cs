using UnityEngine;
using System.Collections.Generic;

public class WorkersManager : MonoBehaviour
{
    public static WorkersManager Instance { get; private set; }
    
    [Header("Worker Prefab")]
    public GameObject workerPrefab;
    
    [Header("Worker Spawn Settings")]
    public Transform workersParent; // Parent transform to organize workers in hierarchy
    public Vector3 spawnPosition = Vector3.zero; // Where workers spawn when hired
    
    [Header("Available Worker Jobs")]
    public List<WorkerJobOption> availableJobs = new List<WorkerJobOption>();
    
    // Track hired workers
    private List<GameObject> hiredWorkers = new List<GameObject>();
    
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
    
    /// <summary>
    /// Attempts to hire a worker with the specified job.
    /// Returns true if successful, false if not enough money or other failure.
    /// </summary>
    public bool TryHireWorker(EmployeeJobData jobData, float cost)
    {
        // Check if player has enough money
        if (UIManager.Instance == null || UIManager.Instance.playerData == null)
        {
            Debug.LogWarning("Cannot hire worker: UIManager or PlayerData not found");
            return false;
        }
        
        PlayerData playerData = UIManager.Instance.playerData;
        
        if (playerData.money < cost)
        {
            Debug.Log("Not enough money to hire worker!");
            return false;
        }
        
        // Deduct money
        playerData.money -= cost;
        UIManager.Instance.UpdateMoney();
        
        // Spawn worker
        GameObject worker = SpawnWorker(jobData);
        
        if (worker != null)
        {
            hiredWorkers.Add(worker);
            Debug.Log($"Successfully hired worker for {jobData.id}. Cost: ${cost}");
            return true;
        }
        else
        {
            // Refund if spawning failed
            playerData.money += cost;
            UIManager.Instance.UpdateMoney();
            Debug.LogError("Failed to spawn worker!");
            return false;
        }
    }
    
    GameObject SpawnWorker(EmployeeJobData jobData)
    {
        if (workerPrefab == null)
        {
            Debug.LogError("Worker prefab not assigned in WorkersManager!");
            return null;
        }
        
        // Spawn at designated position
        GameObject worker = Instantiate(workerPrefab, spawnPosition, Quaternion.identity);
        
        // Set parent if specified
        if (workersParent != null)
        {
            worker.transform.SetParent(workersParent);
        }
        
        // Assign job to worker
        WorkerAgentController controller = worker.GetComponent<WorkerAgentController>();
        if (controller != null)
        {
            controller.job = jobData;
        }
        else
        {
            Debug.LogError("WorkerAgentController not found on worker prefab!");
            Destroy(worker);
            return null;
        }
        
        return worker;
    }
    
    /// <summary>
    /// Get count of hired workers
    /// </summary>
    public int GetHiredWorkerCount()
    {
        // Clean up any destroyed workers
        hiredWorkers.RemoveAll(w => w == null);
        return hiredWorkers.Count;
    }
    
    /// <summary>
    /// Check if a specific job type is already hired
    /// </summary>
    public bool HasWorkerForJob(EmployeeJobData jobData)
    {
        foreach (GameObject worker in hiredWorkers)
        {
            if (worker == null) continue;
            
            WorkerAgentController controller = worker.GetComponent<WorkerAgentController>();
            if (controller != null && controller.job == jobData)
            {
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// Helper class to define worker job options available for hire
/// </summary>
[System.Serializable]
public class WorkerJobOption
{
    public string jobName;
    public EmployeeJobData jobData;
    public float hireCost = 50f;
    public Sprite workerIcon; // Icon to show in UI
    
    [TextArea]
    public string description;
}