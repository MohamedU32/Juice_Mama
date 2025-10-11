using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    [SerializeField] private GameObject thoughtContainer;
    [SerializeField] private SpriteRenderer thoughtIconRenderer;
    [SerializeField] private Sprite[] thoughtSprites;

    private void Awake()
    {
        if (thoughtContainer == null)
        {
            thoughtContainer = gameObject;
        }
        ClearThought();
    }

    public void ShowThought(int index, float duration = 0f)
    {
        if (index >= 0 && index < thoughtSprites.Length)
        {
            thoughtIconRenderer.sprite = thoughtSprites[index];
            thoughtContainer.SetActive(true);
        }

        if (duration > 0f)
        {
            Invoke(nameof(ClearThought), duration);
        }
    }


    public void ClearThought()
    {
        thoughtContainer.SetActive(false);
    }
}
