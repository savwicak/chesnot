
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedItemPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;

    private ItemSlot selectedSlot;

    public ItemSlot SelectedSlot => selectedSlot;


    public void ShowSelectedItem(ItemSlot slot)
    {
        if (slot == null) return;

        selectedSlot = slot;

        if (itemIcon != null)
        {
            itemIcon.sprite = slot.itemIcon;
            itemIcon.enabled = slot.itemIcon != null;
        }

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
    }


    public void ClearSelectedItem()
    {
        selectedSlot = null;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        gameObject.SetActive(false);
    }
}
