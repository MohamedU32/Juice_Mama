using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


/// Handles all collected fruits and broadcasts changes to any listeners ( UI, juicer).

public class FruitCollectionManager : MonoBehaviour
{
    public static FruitCollectionManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private Transform fruitUIParent;   // Parent layout group
    [SerializeField] private GameObject fruitUIPrefab;  // Prefab with "Icon" and "Count"

    private Dictionary<FruitData, int> fruitCounts = new();
    private Dictionary<FruitData, Text> fruitUITextMap = new();

    public event System.Action OnInventoryChanged; // for UI and Juicer

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    
    /// Adds fruits to the player's collection and updates UI.
    
    public void AddFruit(FruitData fruit, int amount = 1)
    {
        if (fruit == null || amount <= 0) return;

        if (!fruitCounts.ContainsKey(fruit))
        {
            fruitCounts[fruit] = 0;
            CreateFruitUI(fruit);
        }

        fruitCounts[fruit] += amount;
        UpdateFruitUI(fruit);
        OnInventoryChanged?.Invoke(); // notify UI & other systems
    }


    /// Removes fruit from inventory -when delivering to juicer).
    
    public bool RemoveFruit(FruitData fruit, int amount = 1)
    {
        if (!fruitCounts.ContainsKey(fruit) || fruitCounts[fruit] < amount)
            return false;

        fruitCounts[fruit] -= amount;

        if (fruitCounts[fruit] <= 0)
            fruitCounts.Remove(fruit);

        UpdateFruitUI(fruit);
        OnInventoryChanged?.Invoke();
        return true;
    }

    
    /// Returns how many of a specific fruit are collected.
    
    public int GetFruitCount(FruitData fruit)
    {
        return fruitCounts.ContainsKey(fruit) ? fruitCounts[fruit] : 0;
    }

    
    /// Returns a full copy of the inventory for external systems.
    
    public Dictionary<FruitData, int> GetAllFruits()
    {
        return new Dictionary<FruitData, int>(fruitCounts);
    }

    
    /// Creates a new UI entry when a new fruit type is collected.
    
    private void CreateFruitUI(FruitData fruit)
    {
        if (fruitUIPrefab == null || fruitUIParent == null)
        {
            Debug.LogWarning("Fruit UI prefab or parent not assigned!");
            return;
        }

        GameObject uiItem = Instantiate(fruitUIPrefab, fruitUIParent);
        Image icon = uiItem.transform.Find("Icon")?.GetComponent<Image>();
        Text countText = uiItem.transform.Find("Count")?.GetComponent<Text>();

        if (icon) icon.sprite = fruit.icon;
        if (countText) countText.text = "0";

        fruitUITextMap[fruit] = countText;
    }

    
    /// Updates an existing UI text element.
    
    private void UpdateFruitUI(FruitData fruit)
    {
        if (fruitUITextMap.ContainsKey(fruit) && fruitUITextMap[fruit] != null)
        {
            if (fruitCounts.ContainsKey(fruit))
                fruitUITextMap[fruit].text = fruitCounts[fruit].ToString();
            else
                fruitUITextMap[fruit].text = "0";
        }
    }
}
