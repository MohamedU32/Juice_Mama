using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryController : MonoBehaviour
{
    [SerializeField] private StorageController storage;
    [SerializeField] private Transform carryRoot;

    private void OnEnable()
    {
        GameEvents.StorageUpdated += OnStorageUpdated;
        SyncVisuals();
    }

    private void OnStorageUpdated(string storageID)
    {
        if (storage.storageID == storageID)
        {
            SyncVisuals();
        }
    }
    private void OnDisable()
    {
        GameEvents.StorageUpdated -= OnStorageUpdated;
    }
    private void SyncVisuals()
    {
        foreach (Transform t in carryRoot) Destroy(t.gameObject);

        foreach (var entry in storage.GetItems)
        {
            if (entry.item is FruitData f)
                for (int i = 0; i < entry.count; i++)
                {
                    var obj = Instantiate(f.prefab, carryRoot);
                    var fc = obj.GetComponent<FruitController>();
                    if (fc) Destroy(fc);
                }

            if (entry.item is JuiceData j)
                for (int i = 0; i < entry.count; i++)
                    Instantiate(j.prefab, carryRoot);
        }
    }
}
