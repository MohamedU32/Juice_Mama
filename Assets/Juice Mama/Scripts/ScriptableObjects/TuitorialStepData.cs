using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorialStep", menuName = "Tutorial/Step", order = 0)]
public class TutorialStepData : ScriptableObject
{
    [Header("Step Info")]
    [Tooltip("Unique name of this tutorial step. Used by the event system to mark completion.")]
    public string stepName;

    [Tooltip("Instruction text shown to the player for this step.")]
    [TextArea(2, 4)]
    public string instructionText;

    [Header("Target Settings")]
    
    public string targetTag;


    public string targetName;

    [Header("Completion Type")]
    [Tooltip("If true, the step completes automatically when the player gets close to the target.")]
    public bool completesByProximity = false;

    [Header("Completion Status")]
    [Tooltip("Tracks whether this step has been completed.")]
    public bool isCompleted = false;

    [Header("Optional Requirements")]
    [Tooltip("If set, the player must have at least this much money before this step becomes available.")]
    public int requiredMoney = 0;

    [Tooltip("If true, the step will only appear when the player has enough money.")]
    public bool showOnlyWhenAffordable = false;

    /// <summary>
    /// Resets the completion state (called automatically at game start).
    /// </summary>
    public void ResetCompletion()
    {
        isCompleted = false;
    }

    /// <summary>
    /// Marks this step as completed and logs the event.
    /// </summary>
    public void MarkCompleted()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            Debug.Log($"[TutorialStepData] Step marked complete: {stepName}");
        }
    }
}
