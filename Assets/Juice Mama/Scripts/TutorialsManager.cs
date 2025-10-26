using UnityEngine;

public class TutorialsManager : MonoBehaviour
{
    public GameObject player;
    public float detectionRadius = 2f;
    public int tutorialStep = 1;

    [Header("Step (1)")]
    public GameObject firstAppleTree;

    [Header("Step (2)")]
    public UnlockableData appleTreeLock1;

    void Start()
    {
        UIManager.Instance.ShowInstruction("Go to the Farm");
    }

    void Update()
    {
        if (CheckProximity(firstAppleTree) && tutorialStep == 1)
        {
            UIManager.Instance.UpdateInstructions("Unlock your first tree by tapping the yellow arrow.");
            tutorialStep++;
        }

        if (appleTreeLock1 !=null && tutorialStep == 2 && appleTreeLock1.isUnlockedByDefault)
        {
            UIManager.Instance.UpdateInstructions("Wait for the fruits to grow. Then, tap on them to collect them.");
            tutorialStep++;
        }
    }

    private bool CheckProximity (GameObject target)
    {
        Vector3 offset = player.transform.position - target.transform.position;

        //"sqrMagnitude" is used instead of "Magnitude" to avoid Square root Calculations (computationally expensive)
        float sqrDistance = offset.sqrMagnitude;        
        float sqrDetectionRadius = detectionRadius * detectionRadius;

        if (sqrDistance <= sqrDetectionRadius)
        {
            return true;
        }

        return false;
    }
}
