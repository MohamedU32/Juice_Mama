using UnityEngine;

public class TutorialsManager : MonoBehaviour
{
    public GameObject player;
    private StorageController playerStorage;
    public float detectionRadius = 2f;
    public int tutorialStep = 1;

    [Header("Step (1)")]
    public GameObject firstAppleTree;

    [Header("Step (2)")]
    public UnlockableData appleTreeLock1;


    void Start()
    {
        playerStorage = player.GetComponent<StorageController>();
        UIManager.Instance.ShowInstruction("Go to the Farm");
    }

    void Update()
    {
        if (tutorialStep == 1 && firstAppleTree != null && CheckProximity(firstAppleTree))
        {
            UIManager.Instance.UpdateInstructions("Unlock your first tree by tapping the yellow arrow.");
            tutorialStep++;
        }

        if (tutorialStep == 2 && appleTreeLock1 !=null && appleTreeLock1.isUnlockedByDefault)
        {
            UIManager.Instance.UpdateInstructions("Wait for the fruits to grow. Then, tap on them to collect them.");
            tutorialStep++;
        }

        if (tutorialStep == 3 && player && playerStorage.GetFruitCount() > 0)
        {
            UIManager.Instance.UpdateInstructions("Go to the Kitchen.");
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
