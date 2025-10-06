using UnityEngine;
using System.Collections.Generic;

public class TreeView : MonoBehaviour
{
    public FruitData fruitData; // Assign apple, mango, etc.
    private List<GameObject> activeFruits = new();

    public void SpawnFruit(int count, Transform[] spawnPoints)
    {
        ClearAllFruits();

        if (fruitData == null || fruitData.fruitPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Missing fruit data, prefab, or spawn points!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject fruitObj = Instantiate(fruitData.fruitPrefab, point.position, point.rotation, point);

            var fruit = fruitObj.GetComponent<Fruit>();
            if (fruit != null)
            {
                fruit.treeView = this;
                fruit.fruitData = fruitData;
            }

            activeFruits.Add(fruitObj);
        }
    }

    public void RemoveOneFruit(GameObject fruit)
    {
        if (fruit == null || !activeFruits.Contains(fruit)) return;

        activeFruits.Remove(fruit);
        Destroy(fruit);
    }

    public void ClearAllFruits()
    {
        foreach (var fruit in activeFruits)
        {
            if (fruit != null) Destroy(fruit);
        }
        activeFruits.Clear();
    }
}
