using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private GameObject player;
    private PlayerController playerController;
    private StorageController playerStorage;

    [SerializeField] private PlayerData playerData;

    [SerializeField] private TextMeshProUGUI m_fruitText;
    [SerializeField] private TextMeshProUGUI m_juiceText;
    [SerializeField] private TextMeshProUGUI m_moneyText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UpdateMoney();
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerStorage = player.GetComponent<StorageController>();
                if (playerStorage == null)
                {
                    Debug.LogError("StorageController component not found on Player.");
                }
                UpdateFruitCount();
                UpdateJuiceCount();
            }
            else
            {
                Debug.LogError("PlayerController component not found on Player.");
            }
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player is tagged 'Player'.");
        }
    }

    public void UpdateFruitCount()
    {
        m_fruitText.text = "" + playerStorage.GetFruitCount() + "/" + playerController.maxFruitCapacity;
    }

    public void UpdateJuiceCount()
    {
        m_juiceText.text = "" + playerStorage.GetJuiceCount() + "/" + playerController.maxJuiceCapacity;
        Debug.Log("Juice Count Updated");
    }

    public void UpdateMoney()
    {
        m_moneyText.text = "$" + playerData.money;
    }
}
