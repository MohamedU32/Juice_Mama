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

    [Header("Resource Display")]
    [SerializeField] private TextMeshProUGUI fruitText;
    [SerializeField] private TextMeshProUGUI juiceText;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Tutorial Instruction UI")]
    //[SerializeField] private GameObject instructionPanel;
    //[SerializeField] private TextMeshProUGUI instructionText;
    //[SerializeField] private float instructionFadeSpeed = 5f;
    
    private CanvasGroup instructionCanvasGroup;
    private bool instructionVisible = false;
    private GameObject player;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup instruction panel if it exists
        //if (instructionPanel != null)
        //{
        //    instructionCanvasGroup = instructionPanel.GetComponent<CanvasGroup>();
        //    if (instructionCanvasGroup == null)
        //    {
        //        instructionCanvasGroup = instructionPanel.AddComponent<CanvasGroup>();
        //    }
        //    instructionCanvasGroup.alpha = 0f;
        //    instructionPanel.SetActive(true);
        //}
    }

    private void Start()
    {
        Debug.Log("=== UIManager Start() ===");
        
        // Auto-find player references if not assigned in Inspector
        if (playerController == null || playerStorage == null)
        {
            Debug.Log("Auto-finding player references...");
            player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                Debug.Log($"Player found: {player.name}");
                
                if (playerController == null)
                {
                    playerController = player.GetComponent<PlayerController>();
                    if (playerController == null)
                    {
                        Debug.LogError("PlayerController component not found on Player.");
                    }
                    else
                    {
                        Debug.Log("PlayerController found!");
                    }
                }

                if (playerStorage == null)
                {
                    playerStorage = player.GetComponent<StorageController>();
                    if (playerStorage == null)
                    {
                        Debug.LogError("StorageController component not found on Player.");
                    }
                    else
                    {
                        Debug.Log("StorageController found!");
                    }
                }
            }
            else
            {
                Debug.LogError("Player not found! Make sure the player is tagged 'Player'.");
            }
        }
        else
        {
            Debug.Log("Player references already assigned in Inspector");
        }

        // Debug UI references
        Debug.Log($"fruitText assigned: {fruitText != null}");
        Debug.Log($"juiceText assigned: {juiceText != null}");
        Debug.Log($"moneyText assigned: {moneyText != null}");
        Debug.Log($"playerData assigned: {playerData != null}");
        //Debug.Log($"instructionPanel assigned: {instructionPanel != null}");
        //Debug.Log($"instructionText assigned: {instructionText != null}");

        // Initial UI updates
        UpdateMoney();
        UpdateFruitCount();
        UpdateJuiceCount();
        
        Debug.Log("=== UIManager Start() Complete ===");
    }

    private void Update()
    {
        // Handle instruction panel fade animation
        //if (instructionCanvasGroup != null)
        //{
        //    float targetAlpha = instructionVisible ? 1f : 0f;
        //    instructionCanvasGroup.alpha = Mathf.Lerp(
        //        instructionCanvasGroup.alpha, 
        //        targetAlpha, 
        //        Time.deltaTime * instructionFadeSpeed
        //    );
        //}
    }

    // === Resource Updates ===
    public void UpdateFruitCount()
    {
        if (fruitText != null && playerStorage != null && playerController != null)
        {
            fruitText.text = $"{playerStorage.GetFruitCount()}/{playerController.maxFruitCapacity}";
        }
    }

    public void UpdateJuiceCount()
    {
        if (juiceText != null && playerStorage != null && playerController != null)
        {
            juiceText.text = $"{playerStorage.GetJuiceCount()}/{playerController.maxJuiceCapacity}";
            Debug.Log("Juice Count Updated");
        }
    }

    public void UpdateMoney()
    {
        if (moneyText != null && playerData != null)
        {
            moneyText.text = $"${(int)playerData.money}";
        }
    }

    // === Tutorial Instructions ===
    //public void ShowInstruction(string text)
    //{
    //    if (instructionText != null)
    //    {
    //        instructionText.text = text;
    //        instructionVisible = true;
    //        if (instructionPanel != null && !instructionPanel.activeSelf)
    //        {
    //            instructionPanel.SetActive(true);
    //        }
    //    }
    //}

    public void HideInstruction()
    {
        instructionVisible = false;
    }

    // === Utility Methods ===
    public bool AreReferencesValid()
    {
        return playerController != null && playerStorage != null && playerData != null;
    }
}