using System;
using UnityEngine;

public static class TutorialEventSystem
{
    /// <summary>
    /// Raised when a tutorial step is completed.
    /// </summary>
    public static event Action<string> OnStepCompleted;

    /// <summary>
    /// Raised when the entire tutorial is completed.
    /// </summary>
    public static event Action OnTutorialCompleted;

    /// <summary>
    /// Call this when a tutorial step (by name) is finished.
    /// </summary>
    public static void RaiseStepCompleted(string stepName)
    {
        Debug.Log($"[TutorialEventSystem] Step completed event raised: {stepName}");
        OnStepCompleted?.Invoke(stepName);
    }

    /// <summary>
    /// Call this when the whole tutorial is complete.
    /// </summary>
    public static void RaiseTutorialCompleted()
    {
        Debug.Log("[TutorialEventSystem] Tutorial completed event raised!");
        OnTutorialCompleted?.Invoke();
    }
}
