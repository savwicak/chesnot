
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedItemPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button equipButton;

    private ItemSlot selectedSlot;

    // Akses slot yang sedang dipilih
    public ItemSlot SelectedSlot => selectedSlot;

    private void Start()
    {
        equipButton.onClick.AddListener(EquipItem);
    }

    public void ShowSelectedItem(ItemSlot slot)
    {
        if (slot == null) return;

        selectedSlot = slot;

        itemIcon.sprite = slot.itemIcon;
        itemIcon.enabled = slot.itemIcon != null;

        equipButton.interactable = true;

        UpdateEquipButton();

        gameObject.SetActive(true);
    }

    private void EquipItem()
    {
        if (selectedSlot == null) return;

        if (selectedSlot.isEquipped)
        {
            selectedSlot.SetEquipped(false);
            Debug.Log("Unequipped: " + selectedSlot.itemName);
        }
        else
        {
            selectedSlot.SetEquipped(true);
            Debug.Log("Equipped: " + selectedSlot.itemName);
        }

        UpdateEquipButton();
    }

    private void UpdateEquipButton()
    {
        if (equipButton == null || selectedSlot == null) return;

        TMP_Text buttonText =
            equipButton.GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            buttonText.text = selectedSlot.isEquipped
                ? "Unequip"
                : "Equip";
        }
    }

    // Membersihkan panel ketika item habis
    public void ClearSelectedItem()
    {
        selectedSlot = null;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (equipButton != null)
        {
            equipButton.interactable = false;
        }

        gameObject.SetActive(false);
    }
}
