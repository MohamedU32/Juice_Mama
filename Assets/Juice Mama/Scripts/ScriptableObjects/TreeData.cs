using UnityEngine;

[CreateAssetMenu(menuName = "Data/TreeSO")]
public class TreeData : ScriptableObject
{
    public string id;
    public string displayName;
    public int level;
    public FruitData fruitData;
}
