using UnityEngine;
using System.Collections.Generic;
using TMPro.Examples;

public class WorkersManager : MonoBehaviour
{
    public static WorkersManager Instance { get; private set; }
    [SerializeField] private List<EmployeeJobData> jobs;
    [SerializeField] private Transform workerSpawnPoint;
    [SerializeField] private PlayerData playerData;

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
        if (jobs == null || jobs.Count == 0) return;
        var origin = workerSpawnPoint ? workerSpawnPoint.position : transform.position;
        for (int j = 0; j < jobs.Count; j++)
        {
            var job = jobs[j];
            if (job == null) continue;
            if (job.totalHired <= 0) continue;
            if (job.employeePrefab == null) continue;
            for (int i = 0; i < job.totalHired; i++)
            {
                var go = Instantiate(job.employeePrefab, origin, Quaternion.identity, transform);
                var worker = go.GetComponent<WorkerAgentController>();
                if (worker != null)
                {
                    worker.SetJob(job);
                }
            }
        }
    }

    void Update()
    {

    }

    public void HireWorker(EmployeeJobData jobData)
    {
        if (jobData == null) return;
        //if (jobData.totalHired >= 1) return;
        if (playerData == null) return;
        if (playerData.money < jobData.hirePrice) return;
        playerData.money -= jobData.hirePrice;
        jobData.totalHired++;

        var go = Instantiate(jobData.employeePrefab, transform.position + new Vector3(0, 0, 1), Quaternion.identity, transform);
        var worker = go.GetComponent<WorkerAgentController>();
        if (worker != null)
        {
            worker.SetJob(jobData);
        }
        UIManager.Instance.ToggleEmployeePanel(false);
        UIManager.Instance.UpdateMoney();
    }

}
