using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HireWorkersPanel : MonoBehaviour
{
    public static HireWorkersPanel Instance { get; private set; }
    
    [Header("Panel References")]
    [SerializeField] private GameObject panelObject; // The main panel GameObject
    [SerializeField] private Button closeButton;
    [SerializeField] private Button openPanelIcon; // The icon button that opens this panel
    
    [Header("Worker Slot UI Elements")]
    [SerializeField] private WorkerSlotUI[] workerSlots; // Assign Worker1, Worker2, Worker3, Worker4
    
    void Start()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Debug logs to check assignments
        Debug.Log($"HireWorkersPanel Start - panelObject: {panelObject != null}, closeButton: {closeButton != null}, openPanelIcon: {openPanelIcon != null}");
        
        // Setup open panel icon button
        if (openPanelIcon != null)
        {
            openPanelIcon.onClick.RemoveAllListeners(); // Clear any existing listeners
            openPanelIcon.onClick.AddListener(OpenPanel);
            Debug.Log("Open panel icon button listener added!");
        }
        else
        {
            Debug.LogError("HireWorkersPanel: openPanelIcon is NULL! Drag the HireWorker Button into the Inspector.");
        }
        
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
            Debug.Log("Close button listener added!");
        }
        else
        {
            Debug.LogError("HireWorkersPanel: closeButton is NULL!");
        }
        
        // Initialize worker slots
        InitializeWorkerSlots();
        
        // Start panel closed
        ClosePanel();
    }
    
    void InitializeWorkerSlots()
    {
        if (WorkersManager.Instance == null || WorkersManager.Instance.availableJobs == null)
        {
            Debug.LogWarning("WorkersManager not properly set up!");
            return;
        }
        
        var availableJobs = WorkersManager.Instance.availableJobs;
        
        for (int i = 0; i < workerSlots.Length; i++)
        {
            if (i < availableJobs.Count)
            {
                // Setup slot with job data
                workerSlots[i].Setup(availableJobs[i], this);
                workerSlots[i].gameObject.SetActive(true);
            }
            else
            {
                // Hide unused slots
                workerSlots[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void OpenPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(true);
            RefreshWorkerSlots();
        }
    }
    
    public void ClosePanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }
    }
    
    public void TogglePanel()
    {
        if (panelObject != null)
        {
            if (panelObject.activeSelf)
                ClosePanel();
            else
                OpenPanel();
        }
    }
    
    /// <summary>
    /// Called when a worker is hired to refresh all slots
    /// </summary>
    public void RefreshWorkerSlots()
    {
        foreach (var slot in workerSlots)
        {
            if (slot != null && slot.gameObject.activeSelf)
            {
                slot.RefreshState();
            }
        }
    }
    
    /// <summary>
    /// Attempts to hire a worker from a slot
    /// </summary>
    public void OnHireWorkerClicked(WorkerJobOption jobOption)
    {
        if (WorkersManager.Instance == null) return;
        
        // Check if already hired
        if (WorkersManager.Instance.HasWorkerForJob(jobOption.jobData))
        {
            Debug.Log($"Worker for {jobOption.jobName} already hired!");
            return;
        }
        
        // Attempt to hire
        bool success = WorkersManager.Instance.TryHireWorker(jobOption.jobData, jobOption.hireCost);
        
        if (success)
        {
            Debug.Log($"Hired {jobOption.jobName}!");
            RefreshWorkerSlots(); // Update UI
        }
        else
        {
            Debug.Log("Failed to hire worker - not enough money!");
        }
    }
}

/// <summary>
/// Individual worker slot UI component
/// Attach this to each Worker1, Worker2, Worker3, Worker4 GameObject
/// </summary>
[System.Serializable]
public class WorkerSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image workerImage;
    public Button hireButton;
    public TextMeshProUGUI buttonText;
    
    private WorkerJobOption jobOption;
    private HireWorkersPanel parentPanel;
    
    public void Setup(WorkerJobOption job, HireWorkersPanel panel)
    {
        jobOption = job;
        parentPanel = panel;
        
        // Set worker icon
        if (workerImage != null && job.workerIcon != null)
        {
            workerImage.sprite = job.workerIcon;
        }
        
        // Setup button
        if (hireButton != null)
        {
            hireButton.onClick.RemoveAllListeners();
            hireButton.onClick.AddListener(OnButtonClicked);
        }
        
        RefreshState();
    }
    
    public void RefreshState()
    {
        if (jobOption == null || WorkersManager.Instance == null) return;
        
        bool alreadyHired = WorkersManager.Instance.HasWorkerForJob(jobOption.jobData);
        
        if (hireButton != null)
        {
            hireButton.interactable = !alreadyHired;
        }
        
        if (buttonText != null)
        {
            if (alreadyHired)
            {
                buttonText.text = "HIRED";
            }
            else
            {
                buttonText.text = $"${(int)jobOption.hireCost}";
            }
        }
    }
    
    void OnButtonClicked()
    {
        if (parentPanel != null && jobOption != null)
        {
            parentPanel.OnHireWorkerClicked(jobOption);
        }
    }
}