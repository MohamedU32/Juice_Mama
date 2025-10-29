using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] private TreeData treeData;
    private GameObject[] spawnedFruits;
    private bool isWaitingToRespawn = false;
    private bool tutorialNotified = false;

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

        isWaitingToRespawn = false;
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

    public TreeData GetTreeData()
    {
        return treeData;
    }

    public void CollectFruits()
    {
        var fruits = GetComponentsInChildren<FruitController>();

        int collected = 0;

        foreach (var fruit in fruits)
        {
            if (fruit.CollectFruit())
            {
                collected++;
            }
        }
    }
}
