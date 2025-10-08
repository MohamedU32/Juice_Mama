using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public GameObject fruitPrefab;
    public GameObject indicator; // Assign in Inspector
    private GameObject[] spawnedFruits;
    private bool isWaitingToRespawn = false;

    void Start()
    {
        SpawnFruits();
    }

    void Update()
    {
        MonitorFruitStatus();
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
            GameObject fruit = Instantiate(fruitPrefab, spawnPoint.position, Quaternion.identity, transform);
            spawnedFruits[i] = fruit;
        }

        isWaitingToRespawn = false; // Reset flag
    }

    void MonitorFruitStatus()
    {
        bool anyGrown = false;
        bool allGone = true;

        foreach (GameObject fruit in spawnedFruits)
        {
            if (fruit != null && fruit.activeSelf)
            {
                allGone = false;

                FruitScript fruitScript = fruit.GetComponent<FruitScript>();
                if (fruitScript != null && fruitScript.isGrown)
                {
                    anyGrown = true;
                }
            }
        }

        // Update indicator based on grown fruits
        if (indicator != null)
        {
            indicator.SetActive(anyGrown);
        }

        // If all fruits are gone and not already waiting to respawn, start new cycle
        if (allGone && !isWaitingToRespawn)
        {
            isWaitingToRespawn = true;
            Invoke("SpawnFruits", 2f); // Delay before respawning
        }
    }
}