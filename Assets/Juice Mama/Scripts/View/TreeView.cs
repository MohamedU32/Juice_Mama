using UnityEngine;
using System;
using System.Collections.Generic;

public class TreeView : MonoBehaviour
{
    private List<GameObject> activeFruits = new List<GameObject>();

    public void SpawnFruit(int count, GameObject fruitPrefab, Transform[] spawnPoints)
    {
        ClearAllFruits();

        if (fruitPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Invalid fruit prefab or spawn points.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            GameObject fruitObj = Instantiate(fruitPrefab, point.position, point.rotation, point);
            activeFruits.Add(fruitObj);
        }
    }

    public void RemoveOneFruit()
    {
        if (activeFruits.Count == 0) return;

        GameObject fruit = activeFruits[0];
        activeFruits.RemoveAt(0);
        if (fruit != null) Destroy(fruit);
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
