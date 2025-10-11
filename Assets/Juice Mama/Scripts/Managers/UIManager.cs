using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private GameObject player;

    [SerializeField] private TextMeshProUGUI m_fruitText;
    [SerializeField] private TextMeshProUGUI m_juiceText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFruitCount(int fruitCarried, int maxFruitCapacity)
    {
        m_fruitText.text = "" + fruitCarried + "/" + maxFruitCapacity;
    }

    public void UpdateJuiceCount(int juicesCarried, int maxJuiceCapacity)
    {
        m_juiceText.text = "" + juicesCarried + "/" + maxJuiceCapacity;
        Debug.Log("Juice Count Updated");
    }
}
