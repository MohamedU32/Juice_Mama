using UnityEngine;

public class LocksManager : MonoBehaviour
{
    public static LocksManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameEvents.OnItemUnlocked += OnItemUnlocked;
    }

    void OnDisable()
    {
        GameEvents.OnItemUnlocked -= OnItemUnlocked;
    }

    private void OnItemUnlocked(string itemId)
    {
        Debug.Log(itemId);
        switch (itemId)
        {
            // First Sequence (first section)
            case "apple-tree-lock-1":
                GameEvents.OnUnlockableAvailable("apple-juicer-lock");
                break;
            case "apple-juicer-lock":
                GameEvents.OnUnlockableAvailable("apple-juice-stand");
                break;
            case "apple-juice-stand":
                GameEvents.OnUnlockableAvailable("apple-tree-lock-2");
                break;
            case "apple-tree-lock-2":
                GameEvents.OnUnlockableAvailable("orange-gate-lock");
                break;
            case "orange-gate-lock":
                GameEvents.OnUnlockableAvailable("orange-tree-lock-1");
                break;
            // Second section (Orange)
            case "orange-tree-lock-1":
                GameEvents.OnUnlockableAvailable("orange-juicer-lock");
                break;
            case "orange-juicer-lock":
                GameEvents.OnUnlockableAvailable("orange-juice-stand");
                break;
            case "orange-juice-stand":
                GameEvents.OnUnlockableAvailable("orange-tree-lock-2");
                break;
            case "orange-tree-lock-2":
                GameEvents.OnUnlockableAvailable("pineapple-gate-lock");
                break;
            case "pineapple-gate-lock":
                GameEvents.OnUnlockableAvailable("pineapple-tree-lock-1");
                break;
            // Third section (Pineapple)
            case "pineapple-tree-lock-1":
                GameEvents.OnUnlockableAvailable("pineapple-juicer-lock");
                break;
            case "pineapple-juicer-lock":
                GameEvents.OnUnlockableAvailable("pineapple-juice-stand");
                break;
            case "pineapple-juice-stand":
                GameEvents.OnUnlockableAvailable("pineapple-tree-lock-2");
                break;
            case "pineapple-tree-lock-2":
                GameEvents.OnUnlockableAvailable("pear-gate-lock");
                break;
            case "pear-gate-lock":
                GameEvents.OnUnlockableAvailable("pear-tree-lock-1");
                break;
            // Forth section (Pear)
            case "pear-tree-lock-1":
                GameEvents.OnUnlockableAvailable("pear-juicer-lock");
                break;
            case "pear-juicer-lock":
                GameEvents.OnUnlockableAvailable("pear-juice-stand");
                break;
            case "pear-juice-stand":
                GameEvents.OnUnlockableAvailable("pear-tree-lock-2");
                break;
            case "pear-tree-lock-2":
                // 
                break;
            default:
                break;
        }
    }
}
