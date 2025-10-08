using UnityEngine;
using UnityEngine.UI;

public class JuiceProgressUI : MonoBehaviour
{
    [Header("Juice Progress UI")]
    public Image fillBar; // UI Image with 'Fill Method' set to 'Horizontal'
    public int maxFruitsRequired = 10; // Adjust based on how many fruits make a full juice

    private int currentFruits = 0;

    private void OnEnable()
    {
        GameEvents.OnFruitCollected += OnFruitCollected;
    }

    private void OnDisable()
    {
        GameEvents.OnFruitCollected -= OnFruitCollected;
    }

    private void OnFruitCollected(FruitData fruit)
    {
        currentFruits++;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (fillBar == null) return;

        float fill = Mathf.Clamp01((float)currentFruits / maxFruitsRequired);
        fillBar.fillAmount = fill;

        // Optional: add color feedback
        fillBar.color = Color.Lerp(Color.red, Color.green, fill);
    }
}
