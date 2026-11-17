using UnityEngine;
using UnityEngine.InputSystem;

public class ItemThrowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SelectedItemPanel selectedItemPanel;
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer aimLine;

    [Header("Throw Settings")]
    [SerializeField] private GameObject chestnutPrefab;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float spawnDistance = 0.5f;
    [SerializeField] private float lineLength = 2f;

    private bool isAiming;
    private Vector2 aimDirection;

    public bool IsAiming => isAiming;

    private void Start()
    {
        if (aimLine != null)
        {
            aimLine.positionCount = 2;
            aimLine.enabled = false;
            aimLine.useWorldSpace = true;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            StartAiming();
        }

        if (!isAiming) return;

        UpdateAiming();

        if (Keyboard.current.qKey.wasReleasedThisFrame)
        {
            ThrowChestnut();
        }
    }

    private void StartAiming()
    {
        if (selectedItemPanel == null || player == null)
            return;

        ItemSlot slot = selectedItemPanel.SelectedSlot;

        if (slot == null || !slot.isFull)
            return;

        if (slot.itemName != "Chestnut")
            return;

        if (chestnutPrefab == null)
        {
            Debug.LogWarning("Chestnut Prefab belum di-assign!");
            return;
        }

        isAiming = true;
        aimDirection = Vector2.down;

        if (aimLine != null)
            aimLine.enabled = true;

        UpdateAiming();
    }

    private void UpdateAiming()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                -mainCamera.transform.position.z
            )
        );

        Vector2 direction = (mouseWorld - player.position).normalized;

        if (direction.sqrMagnitude > 0.001f)
            aimDirection = direction;

        if (aimLine != null)
        {
            Vector3 start = player.position;
            Vector3 end = start + (Vector3)(aimDirection * lineLength);

            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }
    }

    private void ThrowChestnut()
    {
        isAiming = false;

        if (aimLine != null)
            aimLine.enabled = false;

        ItemSlot slot = selectedItemPanel.SelectedSlot;

        if (slot == null || !slot.isFull || slot.itemName != "Chestnut")
            return;

        Vector3 spawnPosition = player.position +
            (Vector3)(aimDirection * spawnDistance);

        GameObject chestnut = Instantiate(
            chestnutPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Rigidbody2D rb = chestnut.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(
                aimDirection * throwForce,
                ForceMode2D.Impulse
            );
        }
        else
        {
            Debug.LogWarning("Chestnut Prefab membutuhkan Rigidbody2D!");
        }

        slot.ConsumeOne();

        if (slot.itemQuantity <= 0)
        {
            selectedItemPanel.ClearSelectedItem();
        }
        else
        {
            selectedItemPanel.ShowSelectedItem(slot);
        }

        Debug.Log("Chestnut Thrown!");
    }
}

