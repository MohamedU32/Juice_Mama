using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fruit", fileName = "NewFruitData")]
public class FruitData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string displayName;

    [Header("Visuals")]
    public Sprite icon;
    public GameObject fruitPrefab;

    [Header("Gameplay Settings")]
    public int value = 1;
    public float regrowTime = 10f;
}