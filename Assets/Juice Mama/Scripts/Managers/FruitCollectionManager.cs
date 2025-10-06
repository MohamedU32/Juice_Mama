using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages all collected fruits and updates the UI dynamically.
/// </summary>
public class FruitCollectionManager : MonoBehaviour
{
    public static FruitCollectionManager Instance { get; private set; }

    [Header("UI Settings")]
    public Transform fruitUIParent;    // Parent object for UI entries (e.g., HorizontalLayoutGroup)
    public GameObject fruitUIPrefab;   // Prefab with Image + Text (icon + count)

    // Tracks collected fruits
    private Dictionary<FruitData, int> fruitCounts = new();
    private Dictionary<FruitData, Text> fruitUITextMap = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Add fruits to the inventory and update the UI.
    /// </summary>
    public void AddFruit(FruitData fruit, int amount = 1)
    {
        if (fruit == null) return;

        // If first time collecting this fruit, create a UI element
        if (!fruitCounts.ContainsKey(fruit))
        {
            fruitCounts[fruit] = 0;
            CreateFruitUI(fruit);
        }

        fruitCounts[fruit] += amount;
        UpdateFruitUI(fruit);
    }

    /// <summary>
    /// Creates a new UI element for the fruit.
    /// </summary>
    private void CreateFruitUI(FruitData fruit)
    {
        if (fruitUIPrefab == null || fruitUIParent == null)
        {
            Debug.LogWarning("Fruit UI prefab or parent is not assigned!");
            return;
        }

        GameObject uiItem = Instantiate(fruitUIPrefab, fruitUIParent);

        Image icon = uiItem.transform.Find("Icon")?.GetComponent<Image>();
        Text countText = uiItem.transform.Find("Count")?.GetComponent<Text>();

        if (icon != null) icon.sprite = fruit.icon;
        if (countText != null) countText.text = "0";

        fruitUITextMap[fruit] = countText;
    }

    /// <summary>
    /// Updates the UI text for the fruit.
    /// </summary>
    private void UpdateFruitUI(FruitData fruit)
    {
        if (fruitUITextMap.ContainsKey(fruit) && fruitUITextMap[fruit] != null)
        {
            fruitUITextMap[fruit].text = fruitCounts[fruit].ToString();
        }
    }

    /// <summary>
    /// Returns how many of this fruit the player has collected.
    /// </summary>
    public int GetFruitCount(FruitData fruit)
    {
        return fruitCounts.ContainsKey(fruit) ? fruitCounts[fruit] : 0;
    }
}
