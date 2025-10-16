using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] private TreeData treeData;
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
            GameObject fruit = Instantiate(treeData.fruitData.prefab, spawnPoint.position, Quaternion.identity, transform);
            spawnedFruits[i] = fruit;
        }

        isWaitingToRespawn = false; // Reset flag
    }

    void MonitorFruitStatus()
    {
        bool allGone = true;

        foreach (GameObject fruit in spawnedFruits)
        {
            if (fruit != null && fruit.activeSelf)
            {
                allGone = false;
            }
        }
        if (allGone && !isWaitingToRespawn)
        {
            isWaitingToRespawn = true;
            Invoke("SpawnFruits", 2f);
        }
    }
}