
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailsPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemIcon;

    [Header("Mouse Settings")]
    [SerializeField] private Vector2 mouseOffset = new Vector2(15f, -15f);

    private RectTransform panelRect;
    private RectTransform canvasRect;
    private Canvas canvas;

    private void Awake()
    {
        panelRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
            canvasRect = canvas.transform as RectTransform;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
            FollowMouse();
    }

    private void FollowMouse()
    {
        if (canvas == null || panelRect == null || canvasRect == null)
            return;

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            uiCamera,
            out Vector2 localMousePosition))
        {
            panelRect.localPosition = localMousePosition + mouseOffset;
        }
    }

    public void ShowDetails(ItemSlot slot)
    {
        if (slot == null)
        {
            Debug.LogError("ItemSlot NULL!");
            return;
        }

        if (slot.ItemDetails == null)
        {
            Debug.LogError("ItemDetails NULL pada slot: " + slot.name);
            return;
        }

        ItemDetails details = slot.ItemDetails;

        if (itemNameText == null)
        {
            Debug.LogError("Item Name Text belum di-assign!");
            return;
        }

        if (descriptionText == null)
        {
            Debug.LogError("Description Text belum di-assign!");
            return;
        }

        if (quantityText == null)
        {
            Debug.LogError("Quantity Text belum di-assign!");
            return;
        }

        if (itemIcon == null)
        {
            Debug.LogError("Item Icon belum di-assign!");
            return;
        }

        itemNameText.text = details.ItemName;
        descriptionText.text = details.Description;
        quantityText.text = "x" + slot.itemQuantity;

        itemIcon.sprite = details.ItemIcon;
        itemIcon.enabled = details.ItemIcon != null;

        gameObject.SetActive(true);
    }

    public void HideDetails()
    {
        gameObject.SetActive(false);
    }
}
