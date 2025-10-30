using TMPro;
using UnityEngine;

public class FridgeController : MonoBehaviour
{
    [Header("Counters")]
    [SerializeField] private TextMeshPro appleCountText;
    [SerializeField] private TextMeshPro orangeCountText;
    [SerializeField] private TextMeshPro pineappleCountText;
    [SerializeField] private TextMeshPro pearCountText;
    private StorageController storage;
    void Start()
    {
        storage = GetComponent<StorageController>();
    }
    void OnEnable()
    {
        GameEvents.OnFridgeLoaded += OnFridgeLoaded;
    }

    void OnDisable()
    {
        GameEvents.OnFridgeLoaded -= OnFridgeLoaded;
    }
    void OnFridgeLoaded()
    {
        UpdateJuiceCounts();
    }

    private void UpdateJuiceCounts()
    {
        var juices = storage.GetAllJuices();
        int appleCount = 0;
        int orangeCount = 0;
        int pineappleCount = 0;
        int pearCount = 0;
        foreach (var juice in juices)
        {
            switch (juice.id)
            {
                case "apple-juice":
                    appleCount = storage.GetCount(juice);
                    break;
                case "orange-juice":
                    orangeCount = storage.GetCount(juice);
                    break;
                case "pineapple-juice":
                    pineappleCount = storage.GetCount(juice);
                    break;
                case "pear-juice":
                    pearCount = storage.GetCount(juice);
                    break;
            }
        }
        appleCountText.text = appleCount.ToString();
        orangeCountText.text = orangeCount.ToString();
        pineappleCountText.text = pineappleCount.ToString();
        pearCountText.text = pearCount.ToString();
    }
}