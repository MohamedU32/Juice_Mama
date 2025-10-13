using TMPro;
using UnityEngine;

public class UnlockController : MonoBehaviour
{
    [SerializeField] private UnlockableData unlockable;
    [SerializeField] private GameObject lockObject;
    [SerializeField] private TextMeshPro costText;

    [SerializeField] private PlayerData playerData;

    bool isUnlocked;

    void Start()
    {
        isUnlocked = unlockable.isUnlockedByDefault;
        lockObject.SetActive(!isUnlocked);
        gameObject.SetActive(isUnlocked);
        if (costText != null)
            costText.text = unlockable.unlockCost.ToString();
    }

    public void TryUnlock()
    {
        if (isUnlocked) return;
        if (playerData.money >= unlockable.unlockCost)
        {
            playerData.money -= unlockable.unlockCost;
            UIManager.Instance.UpdateMoney();
            isUnlocked = true;
            lockObject.SetActive(false);
            gameObject.SetActive(true);
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioNames.FAILED_COLLECTION);
        }
    }
}