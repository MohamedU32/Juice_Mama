using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fruit")]
public class FruitData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject fruitPrefab;
}
