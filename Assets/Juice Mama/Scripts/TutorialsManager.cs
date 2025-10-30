using System.Collections;
using UnityEngine;

public class TutorialsManager : MonoBehaviour
{
    public GameObject player;
    private StorageController playerStorage;
    public PlayerData playerData;
    public GameObject Arrow;
    public GameObject arrowTarget;
    public float detectionRadius = 2f;
    public float currentPlayerMoney = 0;
    public Vector3 currentPlayerPosition = Vector3.zero;
    public int tutorialStep = 1;

    [Header("Step (1)")]
    public GameObject firstAppleTree;

    [Header("Step (2)")]
    public UnlockableData appleTreeLock1;

    [Header("Step (4)")]
    public GameObject appleJuicer;

    [Header("Step (5)")]
    public UnlockableData appleJuicerLock;

    [Header("Step (7)")]
    public GameObject fridge;

    [Header("Step (9)")]
    public GameObject appleJuiceStand;

    [Header("Step (10)")]
    public UnlockableData appleStandLock;

    [Header("Step (12)")]
    public GameObject firstFarmSeparator ;

    [Header("Step (13)")]
    public UnlockableData orangeGateLock;

    void Start()
    {
        //Arrow.SetActive(true);
        playerStorage = player.GetComponent<StorageController>();
        arrowTarget = Arrow.GetComponent<TutorialsArrow>().target;
        UIManager.Instance.ShowInstruction("Go to the Farm");
    }

    void Update()
    {
        if (tutorialStep == 1 && firstAppleTree != null && CheckProximity(firstAppleTree))
        {
            UIManager.Instance.UpdateInstructions("Unlock your first tree by tapping the yellow arrow.");
            arrowTarget = firstAppleTree;
            tutorialStep++;
        }

        if (tutorialStep == 2 && appleTreeLock1 !=null && appleTreeLock1.isUnlockedByDefault)
        {
            UIManager.Instance.UpdateInstructions("Wait for the fruits to grow. Then, tap on them to collect them.");
            arrowTarget = firstAppleTree;
            tutorialStep++;
        }

        if (tutorialStep == 3 && playerStorage!= null && playerStorage.GetFruitCount() > 0)
        {
            UIManager.Instance.UpdateInstructions("Go to the Kitchen.");
            arrowTarget = firstAppleTree;
            tutorialStep++;
        }

        if (tutorialStep == 4 && appleJuicer != null && CheckProximity(appleJuicer))
        {
            UIManager.Instance.UpdateInstructions("Unlock your first juicer by tapping the yellow arrow.");
            arrowTarget = appleJuicer;
            tutorialStep++;
        }

        if (tutorialStep == 5 && appleJuicerLock != null && appleJuicerLock.isUnlockedByDefault)
        {
            UIManager.Instance.UpdateInstructions("Tap the yellow arrow above the blender to start creating juice.");
            arrowTarget = appleJuicer;
            tutorialStep++;
        }

        if (tutorialStep == 6 && playerStorage != null && playerStorage.GetFruitCount() == 0)
        {
            UIManager.Instance.UpdateInstructions("Wait for the juice to be ready. Then, tap on them to collect them.");
            arrowTarget = appleJuicer;
            tutorialStep++;
        }

        if (tutorialStep == 7 && playerStorage != null && playerStorage.GetJuiceCount() > 0)
        {
            UIManager.Instance.UpdateInstructions("Go to the fridge.");
            arrowTarget = appleJuicer;
            tutorialStep++;
        }

        if (tutorialStep == 8 && fridge != null && CheckProximity(fridge))
        {
            UIManager.Instance.UpdateInstructions("Put the juice in the fridge by tapping the yellow arrow.");
            arrowTarget = fridge;
            tutorialStep++;
        }

        if (tutorialStep == 9 && playerStorage != null && playerStorage.GetJuiceCount() == 0)
        {
            UIManager.Instance.UpdateInstructions("Go to the apple stand area.");
            arrowTarget = fridge;
            tutorialStep++;
        }

        if (tutorialStep == 10 && appleJuiceStand != null && CheckProximity(appleJuiceStand))
        {
            UIManager.Instance.UpdateInstructions("Unlock your first stand by tapping the yellow arrow.");
            arrowTarget = appleJuiceStand;
            tutorialStep++;
        }

        if (tutorialStep == 11 && appleStandLock != null && appleStandLock.isUnlockedByDefault)
        {
            UIManager.Instance.UpdateInstructions("Wait for your first customer. Then, tap the yellow arrow to sell the juice.");
            arrowTarget = appleJuiceStand;
            tutorialStep++;
            currentPlayerMoney = playerData.money;
        }

        if (tutorialStep == 12 && playerData != null && playerData.money > currentPlayerMoney)
        {
            UIManager.Instance.UpdateInstructions("Go outside to unlock a new section");
            arrowTarget = appleJuiceStand;
            tutorialStep++;
        }

        if (tutorialStep == 13 && firstFarmSeparator != null && CheckProximity(firstFarmSeparator))
        {
            UIManager.Instance.UpdateInstructions("Unlock the new section by tapping the yellow arrow.");
            arrowTarget = firstFarmSeparator;
            tutorialStep++;
        }

        if (tutorialStep == 14 && orangeGateLock != null && orangeGateLock.isUnlockedByDefault)
        {
            UIManager.Instance.UpdateInstructions("Congratulations on Completing the Tutorials. Use the acquired knoweldege to expand your business and increase your earnings.");
            arrowTarget = firstFarmSeparator;
            tutorialStep++;
            currentPlayerPosition = player.transform.position;
        }

        if (tutorialStep == 15 && player.transform != null && (player.transform.position - currentPlayerPosition).sqrMagnitude > 0 )
        {
            UIManager.Instance.UpdateInstructions("To review this tutorial click on the (?) icon.");
            tutorialStep++;
            Arrow.SetActive(false);
            StartCoroutine(HidingInstructions());
        }
    }

    IEnumerator HidingInstructions()
    {
        yield return(new WaitForSeconds(5));
        UIManager.Instance.HideInstruction();
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
