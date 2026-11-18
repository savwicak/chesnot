using TMPro;
using UnityEngine;

public class MainNumberChesnut : MonoBehaviour
{
    [SerializeField] private TMP_Text otherQuantityText;

    public void ShowDetails(ItemSlot slot)
    {
        if (slot == null)
        {
            otherQuantityText.text = "";
            return;
        }

        otherQuantityText.text = "x" + slot.itemQuantity;
    }
}