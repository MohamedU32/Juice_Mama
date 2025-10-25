using UnityEngine;

[CreateAssetMenu(menuName = "Data/UnlockableSO")]
public class UnlockableData : ScriptableObject
{
    public string id;
    public string displayName;
    public int unlockCost;
    public bool isUnlockedByDefault;
    public bool isAvailableByDefault;
}
