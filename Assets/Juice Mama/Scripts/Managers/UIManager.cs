using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player References")]
    private GameObject player;
    private PlayerController playerController;
    private StorageController playerStorage;
    [SerializeField] private PlayerData playerData;

    [Header("Resource Display")]
    [SerializeField] private TextMeshProUGUI m_fruitText;
    [SerializeField] private TextMeshProUGUI m_juiceText;
    [SerializeField] private TextMeshProUGUI m_moneyText;

    [Header("Tutorial Instruction UI")]
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image instructionBackground;
    [SerializeField] private float instructionFadeSpeed = 5f;

    private CanvasGroup instructionCanvasGroup;
    private bool instructionVisible = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        SetupInstructionUI();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerStorage = player.GetComponent<StorageController>();
                if (playerStorage == null)
                    Debug.LogError("StorageController not found on Player.");

                UpdateFruitCount();
                UpdateJuiceCount();
            }
            else
            {
                Debug.LogError("PlayerController not found on Player.");
            }
        }
        else
        {
            Debug.LogError("Player not found! Tag it as 'Player'.");
        }

        UpdateMoney();
    }

    private void Update()
    {
        // Smooth fade for instruction panel
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

    #region Resource Updates

    public void UpdateFruitCount()
    {
        if (m_fruitText != null && playerStorage != null && playerController != null)
            m_fruitText.text = $"{playerStorage.GetFruitCount()}/{playerController.maxFruitCapacity}";
    }

    public void UpdateJuiceCount()
    {
        if (m_juiceText != null && playerStorage != null && playerController != null)
        {
            m_juiceText.text = $"{playerStorage.GetJuiceCount()}/{playerController.maxJuiceCapacity}";
            Debug.Log("Juice Count Updated");
        }
    }

    public void UpdateMoney()
    {
        if (m_moneyText != null && playerData != null)
            m_moneyText.text = "$" + playerData.money;
    }

    #endregion

    #region Tutorial Instruction Display

    private void SetupInstructionUI()
    {
        if (instructionPanel == null)
        {
            CreateInstructionPanel();
        }
        else
        {
            instructionCanvasGroup = instructionPanel.GetComponent<CanvasGroup>();
            if (instructionCanvasGroup == null)
                instructionCanvasGroup = instructionPanel.AddComponent<CanvasGroup>();

            instructionCanvasGroup.alpha = 0;
            instructionPanel.SetActive(true);
        }
    }

    private void CreateInstructionPanel()
    {
        // Find or create a canvas
        Canvas tutorialCanvas = FindObjectOfType<Canvas>();
        if (tutorialCanvas == null)
        {
            GameObject canvasObj = new GameObject("TutorialCanvas");
            tutorialCanvas = canvasObj.AddComponent<Canvas>();
            tutorialCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create panel
        instructionPanel = new GameObject("InstructionPanel");
        instructionPanel.transform.SetParent(tutorialCanvas.transform, false);

        RectTransform panelRect = instructionPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f); // Top center
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -20);
        panelRect.sizeDelta = new Vector2(500, 100);

        // Background
        instructionBackground = instructionPanel.AddComponent<Image>();
        instructionBackground.color = new Color(0, 0, 0, 0.85f);

        // CanvasGroup for fade
        instructionCanvasGroup = instructionPanel.AddComponent<CanvasGroup>();
        instructionCanvasGroup.alpha = 0;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(instructionPanel.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20, 10);
        textRect.offsetMax = new Vector2(-20, -10);

        instructionText = textObj.AddComponent<TextMeshProUGUI>();
        instructionText.fontSize = 20;
        instructionText.color = Color.white;
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.enableWordWrapping = true;

        instructionPanel.SetActive(true);
    }

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

    public void HideInstruction()
    {
        instructionVisible = false;
    }

    public void UpdateInstruction(string text)
    {
        if (instructionText != null && instructionVisible)
            instructionText.text = text;
    }

    #endregion
}
