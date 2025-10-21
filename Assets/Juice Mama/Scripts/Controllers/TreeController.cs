using UnityEditor.Localization.Platform.Android;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] private TreeData treeData;

    private GameObject[] spawnedFruits;
    private bool isWaitingToRespawn = false;
    private bool tutorialNotified = false; //  ensures we trigger the tutorial only once

    void Start()
    {
        SpawnFruits();
    }

    void Update()
    {
        MonitorFruitStatus();
        DetectTouch();
    }

    void SpawnFruits()
    {
        Transform spawnParent = transform.Find("SpawnPoints");
        if (spawnParent == null)
        {
            Debug.LogWarning("SpawnPoints child not found under Tree.");
            return;
        }

        int count = spawnParent.childCount;
        spawnedFruits = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnParent.GetChild(i);
            GameObject fruit = Instantiate(treeData.fruitData.prefab, spawnPoint.position, Quaternion.identity, transform);
            fruit.GetComponent<FruitController>().parentTree = this;
            spawnedFruits[i] = fruit;
        }

        isWaitingToRespawn = false; // Reset flag
    }


    void DetectTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                GameObject touchedObject = hit.collider.gameObject;

                // Check if we hit this tree or one of its fruits and it has grown fruit(s)
                // Case 1: Touched this tree
                if (touchedObject == gameObject)
                {
                    TryCollectAllGrownFruits();
                }

                // Case 2: Touched a fruit belonging to this tree
                FruitController fruit = touchedObject.GetComponent<FruitController>();
                if (fruit != null && fruit.parentTree == this && fruit.isGrown)
                {
                    TryCollectAllGrownFruits();
                }
            }
        }
    }

    void TryCollectAllGrownFruits()
    {
        if (spawnedFruits == null || spawnedFruits.Length == 0) return;

        foreach (GameObject fruitObj in spawnedFruits)
        {
            if (fruitObj == null) continue;

            FruitController fruit = fruitObj.GetComponent<FruitController>();
            if (fruit != null && fruit.isGrown && fruitObj.activeSelf)
            {
                if (PlayerController.Instance.TryCollectFruit(treeData.fruitData))
                {
                    fruit.moveToPlayer = true;
                }
            }
        }

        // Optional: Notify tutorial system
        if (SimpleTutorialManager.Instance != null)
        {
            SimpleTutorialManager.Instance.ManualComplete("CollectFruit");
        }
    }

    void MonitorFruitStatus()
    {
        if (spawnedFruits == null || spawnedFruits.Length == 0) return;

        bool allGone = true;

        foreach (GameObject fruit in spawnedFruits)
        {
            if (fruit != null && fruit.activeSelf)
            {
                allGone = false;
                break;
            }
        }

        if (allGone && !isWaitingToRespawn)
        {
            isWaitingToRespawn = true;

            // Notify tutorial only once when the tree has been fully harvested
            if (!tutorialNotified)
            {
                SimpleTutorialManager.Instance?.ManualComplete("CollectFruit");
                tutorialNotified = true;
            }

            // Respawn fruits after a short delay
            Invoke(nameof(SpawnFruits), 2f);
        }
    }
}
