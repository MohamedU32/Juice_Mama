using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FruitInventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform container;      // Parent object to hold UI elements
    [SerializeField] private GameObject fruitUIPrefab; // Prefab for displaying each fruit type

    private Dictionary<FruitData, TextMeshProUGUI> fruitCounters = new();

    private void OnEnable()
    {
        if (FruitCollectionManager.Instance != null)
            FruitCollectionManager.Instance.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDisable()
    {
        if (FruitCollectionManager.Instance != null)
            FruitCollectionManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        if (FruitCollectionManager.Instance == null || container == null) return;

        // Clear existing UI
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Get all fruits from inventory
        var fruits = FruitCollectionManager.Instance.GetAllFruits();

        // Rebuild UI
        foreach (var entry in fruits)
        {
            var fruit = entry.Key;
            int count = entry.Value;

            var item = Instantiate(fruitUIPrefab, container);
            var icon = item.transform.Find("Icon").GetComponent<Image>();
            var countText = item.transform.Find("Count").GetComponent<TextMeshProUGUI>();

            icon.sprite = fruit.icon;
            countText.text = count.ToString();

            fruitCounters[fruit] = countText;
        }
    }
}
