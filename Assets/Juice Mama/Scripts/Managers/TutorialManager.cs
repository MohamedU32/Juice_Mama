using UnityEngine;
using System.Collections.Generic;
using Kalkatos.DottedArrow;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial Steps")]
    public List<TutorialStepData> steps;

    [Header("References")]
    public TutorialArrow arrow;
    public Transform player;
    public UIManager uiManager;

    [Header("Settings")]
    public bool resetStepsOnStart = true;
    public bool autoAdvanceOnCompletion = true;
    public float proximityCheckDistance = 2f;

    private int currentStepIndex = 0;
    private Transform currentTarget;
    private TutorialStepData currentStep;
    private bool checkingProximity = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-find components
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (uiManager == null)
            uiManager = UIManager.Instance;

        if (arrow == null)
            arrow = FindObjectOfType<TutorialArrow>();
    }

    void Start()
    {
        if (resetStepsOnStart)
        {
            foreach (var step in steps)
                step.ResetCompletion();
        }

        if (steps.Count > 0)
            StartStep(0);
        else
            Debug.LogWarning(" No tutorial steps assigned!");
    }

    void Update()
    {
        // Check proximity to target for auto-completion
        if (checkingProximity && currentTarget != null && player != null)
        {
            float distance = Vector3.Distance(player.position, currentTarget.position);
            if (distance < proximityCheckDistance)
            {
                CompleteCurrentStep();
                checkingProximity = false;
            }
        }

        // Auto-advance when step is completed
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

        // Find target
        currentTarget = FindTargetInScene(currentStep);

        if (currentTarget == null)
        {
            Debug.LogWarning($" Could not find target for: {currentStep.stepName}");
            return;
        }

        // Show instruction
        uiManager?.ShowInstruction(currentStep.instructionText);

        // Show arrow
        if (arrow != null && player != null)
        {
            arrow.SetTargets(player, currentTarget);
            arrow.SetActive(true);
        }

        // Enable proximity checking
        checkingProximity = true;

        Debug.Log($"✓ Tutorial Step {stepIndex + 1}/{steps.Count}: {currentStep.stepName}");
    }

    Transform FindTargetInScene(TutorialStepData step)
    {
        GameObject targetObj = null;

        // Try tag first
        if (!string.IsNullOrEmpty(step.targetTag))
        {
            targetObj = GameObject.FindGameObjectWithTag(step.targetTag);
            if (targetObj != null)
                Debug.Log($"✓ Found by tag: {step.targetTag}");
        }

        // Try name
        if (targetObj == null && !string.IsNullOrEmpty(step.targetName))
        {
            targetObj = GameObject.Find(step.targetName);
            if (targetObj != null)
                Debug.Log($"✓ Found by name: {step.targetName}");
        }

        return targetObj?.transform;
    }

    // ===== PUBLIC API =====

    public void CompleteCurrentStep()
    {
        if (currentStep != null && !currentStep.isCompleted)
        {
            currentStep.isCompleted = true;
            checkingProximity = false;
            Debug.Log($"✓ Completed: {currentStep.stepName}");
        }
    }

    public void CompleteStep(string stepName)
    {
        TutorialStepData step = steps.Find(s => s.stepName == stepName);
        if (step != null && !step.isCompleted)
        {
            step.isCompleted = true;
            Debug.Log($"✓ Completed: {stepName}");

            if (step == currentStep && !autoAdvanceOnCompletion)
                NextStep();
        }
    }

    public void NextStep()
    {
        if (currentStep != null)
            currentStep.isCompleted = true;

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
            CompleteTutorial();
        else
            StartStep(currentStepIndex);
    }

    public void GoToStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < steps.Count)
        {
            currentStepIndex = stepIndex;
            StartStep(stepIndex);
        }
    }

    void CompleteTutorial()
    {
        arrow?.Deactivate();
        uiManager?.HideInstruction();
        checkingProximity = false;
        Debug.Log("Tutorial Complete!");
    }

    // ===== HELPER METHODS =====

    public bool IsTutorialComplete() => currentStepIndex >= steps.Count;
    public int GetCurrentStepIndex() => currentStepIndex;
    public string GetCurrentStepName() => currentStep?.stepName ?? "None";
    public int GetTotalSteps() => steps.Count;
}
