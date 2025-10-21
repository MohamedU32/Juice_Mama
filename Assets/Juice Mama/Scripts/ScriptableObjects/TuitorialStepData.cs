using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorialStep", menuName = "Tutorial/Step")]
public class TutorialStepData : ScriptableObject
{
    [Header("Step Info")]
    public string stepName;
    
    [TextArea(2, 4)]
    public string instructionText;
    
    [Header("Target Settings")]
    public string targetTag; // Use tag to find target in scene
    public string targetName; // Alternative: find by GameObject name
    
    [Header("Completion")]
    public bool isCompleted = false;
    
    [Header("Optional Requirements")]
    public int requiredMoney = 0; // If step needs money
    public bool showOnlyWhenAffordable = false; // Only show when player has enough money
    
    /// <summary>
    /// Reset completion state (called on game start)
    /// </summary>
    public void ResetCompletion()
    {
        isCompleted = false;
    }
}