using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Kalkatos.DottedArrow;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial Steps")]
    public List<TutorialStepData> steps;

    [Header("References")]
    public TutorialArrow arrow; // Your dotted arrow UI
    public Transform player; // Player transform
    public TMP_Text floatingInstruction; // Instruction text (UI) - Fixed at top of screen

    [Header("Instruction Settings")]
    public bool instructionFollowsTarget = false; // Set to FALSE for fixed top bar

    [Header("Settings")]
    public bool resetStepsOnStart = true;
    public bool autoAdvanceOnCompletion = true;

    private int currentStepIndex = 0;
    private Transform currentTarget;
    private TutorialStepData currentStep;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Reset all step completions
        if (resetStepsOnStart)
        {
            foreach (var step in steps)
                step.ResetCompletion();
        }

        // Start first step
        if (steps.Count > 0)
            StartStep(0);
        else
            Debug.LogWarning("No tutorial steps assigned!");
    }

    void Update()
    {
        // Instruction bar stays fixed at top of screen (no need to update position)
       
        if (instructionFollowsTarget && currentTarget != null && floatingInstruction != null && floatingInstruction.gameObject.activeSelf)
        {
            Vector3 worldPos = currentTarget.position + Vector3.up * 2f;
            floatingInstruction.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }

        // Auto-advance when current step is completed
        if (autoAdvanceOnCompletion && currentStep != null && currentStep.isCompleted)
        {
            NextStep();
        }
    }

    void StartStep(int stepIndex)
    {
        if (stepIndex >= steps.Count)
        {
            CompleteTutorial();
            return;
        }

        currentStepIndex = stepIndex;
        currentStep = steps[stepIndex];

        // Find target object in scene
        currentTarget = FindTargetInScene(currentStep);

        if (currentTarget == null)
        {
            Debug.LogWarning($" Could not find target for step: {currentStep.stepName}");
            Debug.LogWarning($"Looking for Tag: '{currentStep.targetTag}' or Name: '{currentStep.targetName}'");
            return;
        }

        // Show instruction text at top of screen (no position update needed)
        if (floatingInstruction != null)
        {
            floatingInstruction.text = currentStep.instructionText;
            floatingInstruction.gameObject.SetActive(true);
        }

        // Show dotted arrow pointing from player to target
        if (arrow != null && player != null)
        {
            Debug.Log($" Arrow Origin (Player): {player.position}");
            Debug.Log($" Arrow Target: {currentTarget.position}");
            arrow.SetTargets(player, currentTarget);
            arrow.SetActive(true);
        }
        else
        {
            if (arrow == null) Debug.LogError(" Arrow is NULL! Assign it in TutorialManager Inspector.");
            if (player == null) Debug.LogError(" Player is NULL! Assign PlayerContainer to TutorialManager Inspector.");
        }

        Debug.Log($" Tutorial Step {stepIndex + 1}/{steps.Count}: {currentStep.stepName}");
    }

    Transform FindTargetInScene(TutorialStepData step)
    {
        GameObject targetObj = null;

        // Try finding by tag first
        if (!string.IsNullOrEmpty(step.targetTag))
        {
            targetObj = GameObject.FindGameObjectWithTag(step.targetTag);
            if (targetObj != null)
            {
                Debug.Log($"✓ Found target by tag: {step.targetTag}");
            }
        }

        // If not found, try by name
        if (targetObj == null && !string.IsNullOrEmpty(step.targetName))
        {
            targetObj = GameObject.Find(step.targetName);
            if (targetObj != null)
            {
                Debug.Log($"✓ Found target by name: {step.targetName}");
            }
        }

        return targetObj?.transform;
    }

    // ===== PUBLIC METHODS - CALL FROM YOUR GAME SCRIPTS =====

    /// <summary>
    /// Complete the current tutorial step
    /// </summary>
    public void CompleteCurrentStep()
    {
        if (currentStep != null && !currentStep.isCompleted)
        {
            currentStep.isCompleted = true;
            Debug.Log($"Completed: {currentStep.stepName}");
        }
    }

    /// <summary>
    /// Complete a specific step by name
    /// </summary>
    public void CompleteStep(string stepName)
    {
        TutorialStepData step = steps.Find(s => s.stepName == stepName);
        if (step != null && !step.isCompleted)
        {
            step.isCompleted = true;
            Debug.Log($" Completed step: {stepName}");

            // If it's the current step, advance immediately
            if (step == currentStep && !autoAdvanceOnCompletion)
            {
                NextStep();
            }
        }
        else if (step == null)
        {
            Debug.LogWarning($" Step not found: {stepName}");
        }
    }

    /// <summary>
    /// Manually advance to next step
    /// </summary>
    public void NextStep()
    {
        // Mark current as completed
        if (currentStep != null)
            currentStep.isCompleted = true;

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

    /// <summary>
    /// Skip to a specific step by index
    /// </summary>
    public void GoToStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < steps.Count)
        {
            currentStepIndex = stepIndex;
            StartStep(stepIndex);
        }
    }

    /// <summary>
    /// Check if player can afford current step (if it has money requirement)
    /// </summary>
    public bool CanAffordCurrentStep(int playerMoney)
    {
        if (currentStep != null)
        {
            return playerMoney >= currentStep.requiredMoney;
        }
        return true;
    }

    /// <summary>
    /// Call this when player's money changes to check if tutorial can continue
    /// </summary>
    public void OnMoneyChanged(int newMoney)
    {
        if (currentStep != null && currentStep.showOnlyWhenAffordable)
        {
            if (newMoney >= currentStep.requiredMoney && currentTarget == null)
            {
                // Player now has enough money, show the step
                StartStep(currentStepIndex);
            }
        }
    }

    void CompleteTutorial()
    {
        if (arrow != null)
            arrow.Deactivate();
        
        if (floatingInstruction != null)
            floatingInstruction.gameObject.SetActive(false);
        
        Debug.Log(" Tutorial Complete!");
    }

    // ===== HELPER METHODS =====

    /// <summary>
    /// Get current step name
    /// </summary>
    public string GetCurrentStepName()
    {
        return currentStep != null ? currentStep.stepName : "None";
    }

    /// <summary>
    /// Check if tutorial is complete
    /// </summary>
    public bool IsTutorialComplete()
    {
        return currentStepIndex >= steps.Count;
    }

    /// <summary>
    /// Get current step index (0-based)
    /// </summary>
    public int GetCurrentStepIndex()
    {
        return currentStepIndex;
    }

    /// <summary>
    /// Get total number of steps
    /// </summary>
    public int GetTotalSteps()
    {
        return steps.Count;
    }

    //  DEBUG HELPERS 

    [ContextMenu("Complete Current Step")]
    void DebugCompleteStep()
    {
        CompleteCurrentStep();
    }

    [ContextMenu("Skip to Next Step")]
    void DebugNextStep()
    {
        NextStep();
    }

    [ContextMenu("Reset Tutorial")]
    void DebugResetTutorial()
    {
        foreach (var step in steps)
            step.ResetCompletion();
        currentStepIndex = 0;
        StartStep(0);
    }

    [ContextMenu("Print Current Step")]
    void DebugPrintCurrentStep()
    {
        if (currentStep != null)
        {
            Debug.Log($"Current Step {currentStepIndex + 1}/{steps.Count}: {currentStep.stepName}");
            Debug.Log($"Instruction: {currentStep.instructionText}");
            Debug.Log($"Target Tag: {currentStep.targetTag}");
            Debug.Log($"Is Completed: {currentStep.isCompleted}");
        }
        else
        {
            Debug.Log("No current step (tutorial may be complete)");
        }
    }
}