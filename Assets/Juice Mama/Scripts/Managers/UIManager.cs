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
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI customerCountText;

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPanel;
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

    [SerializeField] private GameObject employeePanel;


    private CanvasGroup instructionCanvasGroup;
    private bool instructionVisible = false;
    private GameObject player;

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

    public void ToggleEmployeePanel(bool show)
    {
        if (employeePanel != null)
            employeePanel.SetActive(show);
    }

    // === Resource Updates ===
    public void UpdateFruitCount()
    {
        var fruits = playerStorage.GetAllFruits();
        foreach (var fruit in fruits)
        {
            switch (fruit.id)
            {
                case "apple":
                    appleFruitText.text = playerStorage.GetCount(fruit).ToString();
                    break;
                case "orange":
                    orangeFruitText.text = playerStorage.GetCount(fruit).ToString();
                    break;
                case "pineapple":
                    pineappleFruitText.text = playerStorage.GetCount(fruit).ToString();
                    break;
                case "pear":
                    pearFruitText.text = playerStorage.GetCount(fruit).ToString();
                    break;
            }
        }
    }

    public void UpdateJuiceCount()
    {
        var juices = playerStorage.GetAllJuices();
        foreach (var juice in juices)
        {
            switch (juice.id)
            {
                case "apple-juice":
                    appleJuiceText.text = playerStorage.GetCount(juice).ToString();
                    break;
                case "orange-juice":
                    orangeJuiceText.text = playerStorage.GetCount(juice).ToString();
                    break;
                case "pineapple-juice":
                    pineappleJuiceText.text = playerStorage.GetCount(juice).ToString();
                    break;
                case "pear-juice":
                    pearJuiceText.text = playerStorage.GetCount(juice).ToString();
                    break;
            }
        }
    }

    public void UpdateMoney()
    {
        if (moneyText != null && playerData != null)
            moneyText.text = $"${(int)playerData.money}";
    }

    // === Customer Management ===
    public void UpdateCustomerCount(int count)
    {
        if (customerCountText != null)
            customerCountText.text = $"{count}";
    }

    public void ShowCustomerLeftNotification()
    {
        if (notificationPanel == null || notificationText == null) return;

        StopAllCoroutines();
        StartCoroutine(ShowNotificationCoroutine(" A customer left unserved!"));
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
    public void UpdateInstructions(string text)
    {
        if (instructionText != null)
        {
            instructionText.text = text;
        }
    }

    public void HideInstruction() => instructionVisible = false;

    // === Utility ===
    public bool AreReferencesValid()
    {
        return playerController != null && playerStorage != null && playerData != null;
    }
}