using UnityEngine;

[CreateAssetMenu(menuName = "Data/JuiceFridge")]
public class JuiceFridgeData : ScriptableObject
{
    public string id;
    public string displayName;
    public int count;
    public JuiceData juiceData;
}
