using TMPro;
using UnityEngine;

public class UnlockController : MonoBehaviour
{
    [SerializeField] private UnlockableData unlockable;
    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject lockerObject;
    [SerializeField] private GameObject unlockedObject;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private bool inversedVisibility = false;


    void OnEnable()
    {
        GameEvents.OnUnlockableAvailable += OnUnlockableAvailable;
    }

    void OnDisable()
    {
        GameEvents.OnUnlockableAvailable -= OnUnlockableAvailable;
    }

    void Start()
    {
        if (costText != null) costText.text = unlockable.unlockCost.ToString();
        UpdateVisuals();
    }

    public void TryUnlock()
    {
        if (unlockable != null && unlockable.isUnlockedByDefault) return;
        if (unlockable == null || !unlockable.isAvailableByDefault) return;
        if (playerData.money >= unlockable.unlockCost)
        {
            playerData.money -= unlockable.unlockCost;
            UIManager.Instance.UpdateMoney();
            unlockable.isUnlockedByDefault = true;
            GameEvents.OnItemUnlocked?.Invoke(unlockable.id);
            UpdateVisuals();
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioNames.FAILED_COLLECTION);
        }
    }

    public void SetAvailable(bool v)
    {
        if (unlockable != null) unlockable.isAvailableByDefault = v;
        UpdateVisuals();
    }

    void OnUnlockableAvailable(string targetId)
    {
        if (unlockable != null && unlockable.id == targetId) SetAvailable(true);
    }

    void UpdateVisuals()
    {
        var showUnlocked = unlockable != null && unlockable.isUnlockedByDefault;
        var available = unlockable != null && unlockable.isAvailableByDefault;
        var showLock = !showUnlocked && available;
        var showLocker = !showUnlocked && !available;
        if (lockObject != null) lockObject.SetActive(showLock);
        if (lockerObject != null) lockerObject.SetActive(showLocker);
        if (unlockedObject != null) unlockedObject.SetActive(inversedVisibility ? !showUnlocked : showUnlocked);
    }
}