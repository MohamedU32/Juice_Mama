using UnityEngine;

[CreateAssetMenu(menuName = "Data/Unlockable")]
public class UnlockableData : ScriptableObject
{
    public string id;
    public string displayName;
    public int unlockCost;
    public bool isUnlockedByDefault;
}
