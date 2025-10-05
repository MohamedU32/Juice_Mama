using UnityEngine;

[CreateAssetMenu(menuName = "Data/Juice")]
public class JuiceData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject juicePrefab;
}
