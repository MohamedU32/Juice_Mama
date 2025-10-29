using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player References (Auto-find if null)")]
    public PlayerController playerController;
    public StorageController playerStorage;
    public PlayerData playerData;

    [Header("General Resource Display")]
    [SerializeField] private TextMeshProUGUI fruitText;
    [SerializeField] private TextMeshProUGUI juiceText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI customerCountText; // ✅ Added: customer counter UI

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPanel;       // ✅ Added: for unserved customer notification
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float notificationDuration = 3f;

    [Header("Per-Fruit Counters")]
    public TextMeshProUGUI appleFruitText;
    public TextMeshProUGUI orangeFruitText;
    public TextMeshProUGUI pineappleFruitText;
    public TextMeshProUGUI pearFruitText;

    [Header("Per-Juice Counters")]
    public TextMeshProUGUI appleJuiceText;
    public TextMeshProUGUI orangeJuiceText;
    public TextMeshProUGUI pineappleJuiceText;
    public TextMeshProUGUI pearJuiceText;

    [Header("Tutorial Instruction UI")]
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private float instructionFadeSpeed = 5f;
    
    private CanvasGroup instructionCanvasGroup;
    private bool instructionVisible = false;
    private GameObject player;

    // === Internal Data Structures ===
    private Dictionary<string, TextMeshProUGUI> fruitTextMap;
    private Dictionary<string, TextMeshProUGUI> juiceTextMap;
    private Dictionary<string, int> fruitCounts = new Dictionary<string, int>();
    private Dictionary<string, int> juiceCounts = new Dictionary<string, int>();

    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup instruction panel
        if (instructionPanel != null)
        {
            instructionCanvasGroup = instructionPanel.GetComponent<CanvasGroup>();
            if (instructionCanvasGroup == null)
                instructionCanvasGroup = instructionPanel.AddComponent<CanvasGroup>();

            instructionCanvasGroup.alpha = 0f;
            instructionPanel.SetActive(true);
        }

        // Setup mappings
        fruitTextMap = new Dictionary<string, TextMeshProUGUI>
        {
            {"Apple", appleFruitText},
            {"Orange", orangeFruitText},
            {"Pineapple", pineappleFruitText},
            {"Pear", pearFruitText}
        };

        juiceTextMap = new Dictionary<string, TextMeshProUGUI>
        {
            {"Apple", appleJuiceText},
            {"Orange", orangeJuiceText},
            {"Pineapple", pineappleJuiceText},
            {"Pear", pearJuiceText}
        };

        // Initialize all counts
        foreach (var key in fruitTextMap.Keys)
        {
            fruitCounts[key] = 0;
            juiceCounts[key] = 0;
            UpdateFruitText(key);
            UpdateJuiceText(key);
        }

        // Hide notification by default
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    private void Start()
    {
        // Auto-find player references
        if (playerController == null || playerStorage == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (playerController == null)
                    playerController = player.GetComponent<PlayerController>();
                if (playerStorage == null)
                    playerStorage = player.GetComponent<StorageController>();
            }
        }

        UpdateMoney();
        UpdateFruitCount();
        UpdateJuiceCount();
    }

    private void Update()
    {
        // Fade instructions
        if (instructionCanvasGroup != null)
        {
            float targetAlpha = instructionVisible ? 1f : 0f;
            instructionCanvasGroup.alpha = Mathf.Lerp(
                instructionCanvasGroup.alpha,
                targetAlpha,
                Time.deltaTime * instructionFadeSpeed
            );
        }
    }

    // === Resource Updates ===
    public void UpdateFruitCount()
    {
        if (fruitText != null && playerStorage != null && playerController != null)
            fruitText.text = $"{playerStorage.GetFruitCount()}/{playerController.maxFruitCapacity}";
    }

    public void UpdateJuiceCount()
    {
        if (juiceText != null && playerStorage != null && playerController != null)
            juiceText.text = $"{playerStorage.GetJuiceCount()}/{playerController.maxJuiceCapacity}";
    }

    public void UpdateMoney()
    {
        if (moneyText != null && playerData != null)
            moneyText.text = $"${(int)playerData.money}";
    }

    // === Per-type Fruit/Juice Updates ===
    public void AddFruit(string type)
    {
        if (!fruitCounts.ContainsKey(type)) return;
        fruitCounts[type]++;
        UpdateFruitText(type);
    }

    public void AddJuice(string type)
    {
        if (!juiceCounts.ContainsKey(type)) return;
        juiceCounts[type]++;
        UpdateJuiceText(type);
    }

    private void UpdateFruitText(string type)
    {
        if (fruitTextMap[type] != null)
            fruitTextMap[type].text = $"{fruitCounts[type]}/3";
    }

    private void UpdateJuiceText(string type)
    {
        if (juiceTextMap[type] != null)
            juiceTextMap[type].text = $"{juiceCounts[type]}/3";
    }

    public void ResetCounts()
    {
        foreach (var key in fruitCounts.Keys)
        {
            fruitCounts[key] = 0;
            juiceCounts[key] = 0;
            UpdateFruitText(key);
            UpdateJuiceText(key);
        }
    }

    // === Customer Management ===
    public void UpdateCustomerCount(int count)
    {
        if (customerCountText != null)
            customerCountText.text = $"Customers: {count}";
    }

    public void ShowCustomerLeftNotification()
    {
        if (notificationPanel == null || notificationText == null) return;

        StopAllCoroutines();
        StartCoroutine(ShowNotificationCoroutine("⚠️ A customer left unserved!"));
    }

    private System.Collections.IEnumerator ShowNotificationCoroutine(string message)
    {
        notificationPanel.SetActive(true);
        notificationText.text = message;
        yield return new WaitForSeconds(notificationDuration);
        notificationPanel.SetActive(false);
    }

    // === Tutorial UI ===
    public void ShowInstruction(string text)
    {
        if (instructionText != null)
        {
            instructionText.text = text;
            instructionVisible = true;
            if (instructionPanel != null && !instructionPanel.activeSelf)
                instructionPanel.SetActive(true);
        }
    }

    public void HideInstruction() => instructionVisible = false;

    // === Utility ===
    public bool AreReferencesValid()
    {
        return playerController != null && playerStorage != null && playerData != null;
    }
}
