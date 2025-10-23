using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject loadingImage; // Your loading image
    
    [Header("Settings")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private float pulseDuration = 5f; // How long to pulse (5 seconds)
    [SerializeField] private float pulseSpeed = 2f; // How fast it pulses
    [SerializeField] private float pulseScale = 1.2f; // How much bigger it gets (1.2 = 20% bigger)
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource buttonClickSound;
    
    void Start()
    {
        //  loading panel is hidden at start
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
        
        // Connect play button
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }
        
        Debug.Log("Title Screen Loaded Successfully!");
    }
    
    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked! Loading game...");
        PlayButtonSound();
        
        // Show loading panel
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        
        // Hide play button
        if (playButton != null)
            playButton.gameObject.SetActive(false);
        
        // Start loading with pulse animation
        StartCoroutine(LoadGameWithPulse());
    }
    
    IEnumerator LoadGameWithPulse()
    {
        // Start the pulse animation
        if (loadingImage != null)
        {
            StartCoroutine(PulseLoadingImage());
        }
        
        // Wait for the pulse duration (5 seconds)
        yield return new WaitForSeconds(pulseDuration);
        
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }
    
    IEnumerator PulseLoadingImage()
    {
        if (loadingImage == null) yield break;
        
        Vector3 originalScale = loadingImage.transform.localScale;
        float elapsedTime = 0f;
        
        // Pulse for the specified duration
        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calculate pulse using sine wave for smooth in-out effect
            float pulse = Mathf.Sin(elapsedTime * pulseSpeed * Mathf.PI) * (pulseScale - 1f) + 1f;
            
            // Apply the pulse scale
            loadingImage.transform.localScale = originalScale * pulse;
            
            yield return null;
        }
        
        // Reset to original scale when done
        loadingImage.transform.localScale = originalScale;
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    private void PlayButtonSound()
    {
        if (buttonClickSound != null)
        {
            buttonClickSound.Play();
        }
    }
    
    void Update()
    {
        // Android back button handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
}