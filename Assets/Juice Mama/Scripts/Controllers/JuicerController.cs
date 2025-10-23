using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuicerController : MonoBehaviour
{
    [SerializeField] private JuicerData juicerData;
    private Transform spawnPointsParent;
    [SerializeField] private Transform fruitsSpawnLocation;
    [SerializeField] private Transform juicesSpawnLocation;

    private List<Transform> spawnPoints = new List<Transform>();
    private StorageController juicerStorage;

    private bool isProcessing = false;

    private void Start()
    {
        spawnPointsParent = transform.Find("SpawnPoints");
        juicerStorage = gameObject.GetComponent<StorageController>();
        if (juicerStorage == null)
        {
            Debug.LogError("StorageController component not found on Juicer.");
        }

        if (spawnPointsParent != null)
        {
            foreach (Transform child in spawnPointsParent)
                spawnPoints.Add(child);
        }
        else
        {
            Debug.LogWarning("SpawnPoints object not found under JuicingMachine.");
        }
    }

    private void Update()
    {
        if (isProcessing) return;
        ProcessJuice();
    }

    private IEnumerator SpawnJuicePackets(int count)
    {
        isProcessing = true;

        int maxBatches = count;
        foreach (RecipeEntry recipeItem in juicerData.recipe)
        {
            int available = juicerStorage.GetCount(recipeItem.fruitData) / recipeItem.count;
            if (available < maxBatches) maxBatches = available;
        }

        for (int i = 0; i < maxBatches; i++)
        {
            AudioManager.Instance.PlaySound(AudioNames.JUICER_PROCESSING, 1.0f);
            yield return new WaitForSeconds(juicerData.processTime);

            foreach (RecipeEntry recipeItem in juicerData.recipe)
                juicerStorage.Remove(recipeItem.fruitData, recipeItem.count);

            juicerStorage.Add(juicerData.juiceData, 1);
        }

        // ✅ Notify tutorial that juice production is complete
        TutorialEventSystem.RaiseStepCompleted("MakeJuice");

        SyncVisuals();
        isProcessing = false;
    }

    public void PickJuice(StorageController targetStorage, int count)
    {
        if (juicerStorage == null || targetStorage == null) return;

        int juiceCount = juicerStorage.GetCount(juicerData.juiceData);
        if (juiceCount > 0)
        {
            int transferred = juicerStorage.TransferItemsTo(targetStorage, juicerData.juiceData, count);
            if (transferred > 0)
            {
                SyncVisuals();
            }
        }
    }

    private bool hasFruitsForRecipe()
    {
        if (juicerData.recipe.Count == 0) return false;

        foreach (RecipeEntry recipeItem in juicerData.recipe)
        {
            int availableCount = juicerStorage.GetCount(recipeItem.fruitData);
            if (availableCount < recipeItem.count)
                return false;
        }
        return true;
    }

    public void FillStorage(StorageController sourceStorage)
    {
        if (juicerStorage == null || sourceStorage == null) return;

        foreach (RecipeEntry recipeItem in juicerData.recipe)
        {
            int needed = juicerData.maxPerFruit - juicerStorage.GetCount(recipeItem.fruitData);
            if (needed > 0)
            {
                int transferred = sourceStorage.TransferItemsTo(juicerStorage, recipeItem.fruitData, needed);
                UIManager.Instance.UpdateFruitCount();
                UIManager.Instance.UpdateJuiceCount();

                if (transferred > 0)
                    AudioManager.Instance.PlaySound(AudioNames.STORAGE_FILLED, 1.0f);
            }
        }

        SyncVisuals();
    }

    public void ProcessJuice()
    {
        if (juicerStorage == null) return;
        if (!hasFruitsForRecipe()) return;

        StartCoroutine(SpawnJuicePackets(juicerData.outputCount));
    }

    void SyncVisuals()
    {
        foreach (Transform t in fruitsSpawnLocation) Destroy(t.gameObject);
        foreach (Transform t in juicesSpawnLocation) Destroy(t.gameObject);

        foreach (var entry in juicerStorage.GetItems)
        {
            if (entry.item is FruitData f)
                for (int i = 0; i < entry.count; i++)
                {
                    var obj = Instantiate(f.prefab, fruitsSpawnLocation);
                    var fc = obj.GetComponent<FruitController>();
                    if (fc) Destroy(fc);
                }

            if (entry.item is JuiceData j)
                for (int i = 0; i < entry.count; i++)
                    Instantiate(j.prefab, juicesSpawnLocation);
        }
    }
}
