using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    public void UpdateCountText(string text)
    {
        countText.text = text;
    }
}

