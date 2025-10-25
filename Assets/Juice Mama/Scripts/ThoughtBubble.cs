using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    [SerializeField] private GameObject thoughtContainer;
    [SerializeField] private SpriteRenderer thoughtIconRenderer;

    private void Awake()
    {
        if (thoughtContainer == null)
        {
            thoughtContainer = gameObject;
        }
        ClearThought();
    }

    public void ShowThought(int index, float duration = 0f, Sprite thoughtSprite = null)
    {
        if (thoughtSprite != null)
        {
            thoughtIconRenderer.sprite = thoughtSprite;
            thoughtContainer.SetActive(true);
            if (duration > 0f)
            {
                Invoke(nameof(ClearThought), duration);
            }
            return;
        }
    }


    public void ClearThought()
    {
        thoughtContainer.SetActive(false);
    }
}
