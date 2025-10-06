using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fruit", fileName = "NewFruitData")]
public class FruitData : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Unique ID for this fruit type (e.g., 'apple', 'mango').")]
    public string id;

    [Tooltip("Display name for this fruit (e.g., 'Apple').")]
    public string displayName;

    [Header("Visuals")]
    [Tooltip("Icon used in the UI (inventory, counters, etc.).")]
    public Sprite icon;

    [Tooltip("Prefab of the fruit to spawn on the tree.")]
    public GameObject fruitPrefab;

    [Header("Gameplay Settings")]
    [Tooltip("How much value or score this fruit adds when collected.")]
    public int value = 1;

    [Tooltip("Time it takes for this fruit to regrow after being collected.")]
    public float regrowTime = 10f;
}
