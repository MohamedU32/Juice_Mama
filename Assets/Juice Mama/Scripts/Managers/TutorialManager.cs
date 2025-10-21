using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorialManager : MonoBehaviour
{
    public static SimpleTutorialManager Instance;
    public List<TutorialStepData> steps;
    public TutorialArrow3D arrow;
    public UIManager uiManager;
    public Transform player;
    public float proximityDistance = 2f;

    private int currentStepIndex = 0;
    private Transform currentTarget;
    private bool active = false;
    private bool stepCompleted = false; // Prevents double completion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (uiManager == null) uiManager = UIManager.Instance;
        if (arrow == null) arrow = FindObjectOfType<TutorialArrow3D>();
    }

    private void Start()
    {
        foreach (var step in steps) step.ResetCompletion();
        if (steps.Count > 0) StartStep(0);
    }

    private void Update()
    {
        if (!active || currentTarget == null || player == null || stepCompleted) return;

        var currentStep = steps[currentStepIndex];
        
        // Only auto-complete based on proximity for movement steps
        if (currentStep.stepName.Contains("GoTo") || currentStep.stepName.Contains("Walk") || currentStep.stepName.Contains("Move"))
        {
            if (Vector3.Distance(player.position, currentTarget.position) < proximityDistance)
            {
                Debug.Log($"Proximity complete: {currentStep.stepName}");
                CompleteStep();
            }
        }
    }

    private void StartStep(int index)
    {
        if (index >= steps.Count)
        {
            CompleteTutorial();
            return;
        }

        currentStepIndex = index;
        stepCompleted = false; // Reset flag for new step
        var step = steps[index];

        Debug.Log($" Starting Step {index}: '{step.stepName}' - {step.instructionText}");

        // Find target
        currentTarget = null;
        
        if (!string.IsNullOrEmpty(step.targetTag))
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag(step.targetTag);
            currentTarget = targetObj?.transform;
            Debug.Log($"Found target by tag '{step.targetTag}': {(targetObj != null ? targetObj.name : "NULL")}");
        }
        
        if (currentTarget == null && !string.IsNullOrEmpty(step.targetName))
        {
            GameObject targetObj = GameObject.Find(step.targetName);
            currentTarget = targetObj?.transform;
            Debug.Log($"Found target by name '{step.targetName}': {(targetObj != null ? targetObj.name : "NULL")}");
        }

        if (currentTarget == null)
        {
            Debug.LogWarning($" Cannot find target for step: {step.stepName}. Continuing without arrow...");
            // Don't skip - just show instruction without arrow
        }

        // Update UI and arrow
        if (uiManager != null)
        {
            uiManager.ShowInstruction(step.instructionText);
            Debug.Log($" Instruction shown: {step.instructionText}");
        }
        
        if (arrow != null && currentTarget != null)
        {
            arrow.SetTarget(currentTarget);
            Debug.Log($" Arrow pointing to: {currentTarget.name} at {currentTarget.position}");
        }
        else if (arrow != null)
        {
            arrow.Hide(); // Hide arrow if no target
            Debug.Log(" Arrow hidden (no target)");
        }
        
        active = true;
    }

    private void CompleteStep()
    {
        if (stepCompleted) return; // Prevent double completion
        
        stepCompleted = true;
        var step = steps[currentStepIndex];
        step.isCompleted = true;
        Debug.Log($" Step {currentStepIndex} Complete: '{step.stepName}'");
        
        // Small delay before next step for better UX
        Invoke(nameof(NextStep), 0.5f);
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
            StartStep(currentStepIndex);
        }
    }

    private void CompleteTutorial()
    {
        active = false;
        
        if (arrow != null)
        {
            arrow.Hide();
            Debug.Log("Arrow hidden - Tutorial complete");
        }
        
        if (uiManager != null)
        {
            uiManager.HideInstruction();
            Debug.Log("Instructions hidden - Tutorial complete");
        }
        
        Debug.Log(" Tutorial Complete! ");
    }

    public void ManualComplete(string stepName)
    {
        if (currentStepIndex >= steps.Count)
        {
            Debug.LogWarning($"Tutorial already complete, ignoring ManualComplete for: '{stepName}'");
            return;
        }

        if (stepCompleted)
        {
            Debug.LogWarning($"Step already completed, ignoring duplicate ManualComplete for: '{stepName}'");
            return;
        }

        var currentStep = steps[currentStepIndex];
        
        Debug.Log($"ManualComplete called for: '{stepName}' | Current step: '{currentStep.stepName}'");

        // Check if step names match (case-insensitive and trimmed)
        string currentStepName = currentStep.stepName.Trim().ToLower();
        string calledStepName = stepName.Trim().ToLower();

        if (currentStepName == calledStepName)
        {
            Debug.Log($"Manual completion ACCEPTED for: '{stepName}'");
            CompleteStep();
        }
        else
        {
            Debug.LogWarning($" ManualComplete IGNORED. Expected '{currentStep.stepName}' but got '{stepName}'");
            Debug.LogWarning($" stepName in TutorialStepData exactly matches the ManualComplete() call");
        }
    }

    // Helper: Get current step name (useful for debugging)
    public string GetCurrentStepName()
    {
        if (currentStepIndex < steps.Count)
            return steps[currentStepIndex].stepName;
        return "None - Tutorial Complete";
    }

    // Force complete current step (debugging only)
    [ContextMenu("Force Complete Current Step")]
    public void ForceCompleteCurrentStep()
    {
        if (currentStepIndex < steps.Count && !stepCompleted)
        {
            Debug.Log($" FORCED completion of: {steps[currentStepIndex].stepName}");
            CompleteStep();
        }
    }
}