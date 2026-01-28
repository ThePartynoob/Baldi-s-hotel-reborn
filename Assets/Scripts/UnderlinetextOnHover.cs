using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnderlinetextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI buttonText;

    void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.fontStyle |= FontStyles.Underline;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.fontStyle &= ~FontStyles.Underline;
        }
    }
}
