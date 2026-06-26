using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapZoneHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image overlayImage;
    [SerializeField] private TextMeshProUGUI mapNameText;

    [Header("Hover Settings")]
    [SerializeField] private float normalAlpha = 0f;
    [SerializeField] private float hoverAlpha = 0.22f;
    [SerializeField] private float hoverScale = 1.08f;

    private Vector3 originalTextScale;

    private void Awake()
    {
        if (overlayImage == null)
        {
            overlayImage = GetComponent<Image>();
        }

        if (mapNameText != null)
        {
            originalTextScale = mapNameText.transform.localScale;
            mapNameText.gameObject.SetActive(false);
        }

        SetOverlayAlpha(normalAlpha);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetOverlayAlpha(hoverAlpha);

        if (mapNameText != null)
        {
            mapNameText.gameObject.SetActive(true);
            mapNameText.transform.localScale = originalTextScale * hoverScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOverlayAlpha(normalAlpha);

        if (mapNameText != null)
        {
            mapNameText.gameObject.SetActive(false);
            mapNameText.transform.localScale = originalTextScale;
        }
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (overlayImage == null) return;

        Color color = overlayImage.color;
        color.a = alpha;
        overlayImage.color = color;
    }
}