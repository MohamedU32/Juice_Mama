using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorialManager : MonoBehaviour
{
    public static SimpleTutorialManager Instance { get; private set; }

    [Header("Tutorial Configuration")]
    public List<TutorialStepData> steps;
    public float proximityDistance = 2f;

    [Header("References")]
    public TutorialArrow3D arrow;
    public UIManager uiManager;
    public Transform player;

    private int currentStepIndex = 0;
    private Transform currentTarget;
    private bool active = false;
    private bool stepCompleted = false;

    // ----------------------------------------------------
    // LIFECYCLE
    // ----------------------------------------------------
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (uiManager == null)
            uiManager = UIManager.Instance;
        if (arrow == null)
            arrow = FindObjectOfType<TutorialArrow3D>();

        if (player == null)
            Debug.LogError("[TutorialManager] Player not found (tag 'Player').");
        if (uiManager == null)
            Debug.LogError("[TutorialManager] UIManager not found in scene.");
    }

    private void OnEnable()
    {
        TutorialEventSystem.OnStepCompleted += OnStepCompleted;
    }

    private void OnDisable()
    {
        TutorialEventSystem.OnStepCompleted -= OnStepCompleted;
    }

    private void Start()
    {
        if (steps == null || steps.Count == 0)
        {
            Debug.LogWarning("[TutorialManager] No steps defined!");
            return;
        }

        foreach (var step in steps)
            step.ResetCompletion();

        StartStep(0);
    }

    private void Update()
    {
        if (!active || currentTarget == null || player == null || stepCompleted)
            return;

        var currentStep = steps[currentStepIndex];

        // Handle proximity-based completion
        if (currentStep.completesByProximity)
        {
            float distance = Vector3.Distance(player.position, currentTarget.position);
            if (distance < proximityDistance)
            {
                Debug.Log($"[Tutorial] Proximity complete: {currentStep.stepName}");
                CompleteStep();
            }
        }
    }

    // ----------------------------------------------------
    // EVENT HANDLING
    // ----------------------------------------------------
    private void OnStepCompleted(string stepName)
    {
        if (currentStepIndex >= steps.Count) return;

        var currentStep = steps[currentStepIndex];

        if (currentStep.isCompleted)
        {
            Debug.Log($"[Tutorial] Step '{currentStep.stepName}' already marked complete.");
            return;
        }

        if (string.Equals(currentStep.stepName.Trim(), stepName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"[Tutorial] Event completion accepted for '{stepName}'.");
            CompleteStep();
        }
        else
        {
            Debug.Log($"[Tutorial] Event ignored. Expected '{currentStep.stepName}', got '{stepName}'.");
        }
    }

    // ----------------------------------------------------
    // STEP MANAGEMENT
    // ----------------------------------------------------
    private void StartStep(int index)
    {
        if (index >= steps.Count)
        {
            CompleteTutorial();
            return;
        }

        currentStepIndex = index;
        stepCompleted = false;
        var step = steps[index];

        Debug.Log($"[Tutorial] Starting Step {index}: '{step.stepName}' - {step.instructionText}");

        // === Find Target ===
        currentTarget = null;

        if (!string.IsNullOrEmpty(step.targetTag))
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag(step.targetTag);
            currentTarget = targetObj?.transform;
            Debug.Log($"[Tutorial] Found target by tag '{step.targetTag}': {(targetObj ? targetObj.name : "NULL")}");
        }

        if (currentTarget == null && !string.IsNullOrEmpty(step.targetName))
        {
            GameObject targetObj = GameObject.Find(step.targetName);
            currentTarget = targetObj?.transform;
            Debug.Log($"[Tutorial] Found target by name '{step.targetName}': {(targetObj ? targetObj.name : "NULL")}");
        }

        // === Update Instruction UI ===
        if (uiManager != null)
        {
            uiManager.ShowInstruction(step.instructionText);
            Debug.Log($"[Tutorial] Instruction shown: {step.instructionText}");
        }

        // === Update Arrow Target ===
        if (arrow != null)
        {
            if (currentTarget != null)
            {
                arrow.SetTarget(currentTarget);
                arrow.Show();
                Debug.Log($"[Tutorial] Arrow pointing to: {currentTarget.name}");
            }
            else
            {
                arrow.Hide();
                Debug.Log("[Tutorial] Arrow hidden (no valid target)");
            }
        }

        active = true;
    }

    private void CompleteStep()
    {
        var step = steps[currentStepIndex];
        if (step.isCompleted) return;

        step.MarkCompleted();
        stepCompleted = true;

        Debug.Log($"[Tutorial] Step Complete: {step.stepName}");

        uiManager?.HideInstruction();
        arrow?.Hide();

        Invoke(nameof(NextStep), 0.6f);
    }

    private void NextStep()
    {
        currentStepIndex++;
        if (currentStepIndex >= steps.Count)
        {
            CompleteTutorial();
        }
        else
        {
            StartStep(currentStepIndex); // ✅ refreshes UI and arrow each step
        }
    }

    private void CompleteTutorial()
    {
        active = false;
        arrow?.Hide();
        uiManager?.HideInstruction();
        Debug.Log("[Tutorial] All steps complete!");
    }

    // ----------------------------------------------------
    // MANUAL OVERRIDES
    // ----------------------------------------------------
    [ContextMenu("Force Complete Current Step")]
    public void ForceCompleteCurrentStep()
    {
        if (currentStepIndex < steps.Count && !stepCompleted)
        {
            Debug.Log($"[Tutorial] Forced completion of step: {steps[currentStepIndex].stepName}");
            CompleteStep();
        }
    }

    public string GetCurrentStepName()
    {
        return (currentStepIndex < steps.Count)
            ? steps[currentStepIndex].stepName
            : "None - Tutorial Complete";
    }

    // ----------------------------------------------------
    // MANUAL COMPLETION (EXTERNAL TRIGGER)
    // ----------------------------------------------------
    public void ManualComplete(string stepName)
    {
        if (string.IsNullOrEmpty(stepName)) return;

        var step = steps.Find(s =>
            string.Equals(s.stepName.Trim(), stepName.Trim(), StringComparison.OrdinalIgnoreCase));

        if (step != null && !step.isCompleted)
        {
            Debug.Log($"[Tutorial] Manually completing step: {stepName}");
            if (string.Equals(step.stepName, steps[currentStepIndex].stepName, StringComparison.OrdinalIgnoreCase))
                CompleteStep(); // only if it's the current one
        }
        else
        {
            Debug.LogWarning($"[Tutorial] Step '{stepName}' not found or already complete.");
        }
    }
}
