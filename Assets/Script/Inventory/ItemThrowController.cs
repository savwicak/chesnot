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
    [SerializeField] private float spawnDistance = 0.8f;
    [SerializeField] private float lineLength = 2f;

    [Header("Physics")]
    [SerializeField] private float chestnutGravity = 0f;

    private bool isAiming;
    private Vector2 aimDirection = Vector2.down;

    public bool IsAiming => isAiming;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (aimLine != null)
        {
            aimLine.positionCount = 2;
            aimLine.useWorldSpace = true;
            aimLine.enabled = false;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
            StartAiming();

        if (!isAiming)
            return;

        UpdateAiming();

        if (Keyboard.current.qKey.wasReleasedThisFrame)
            ThrowChestnut();
    }

    private void StartAiming()
    {
        if (selectedItemPanel == null || player == null || mainCamera == null)
            return;

        ItemSlot slot = selectedItemPanel.SelectedSlot;

        if (slot == null || !slot.isFull || slot.itemName != "Chestnut")
        {
            Debug.Log("Chestnut belum dipilih!");
            return;
        }

        if (chestnutPrefab == null)
        {
            Debug.LogError("Chestnut Prefab belum di-assign!");
            return;
        }

        isAiming = true;

        if (aimLine != null)
            aimLine.enabled = true;

        UpdateAiming();

        Debug.Log("Started Aiming!");
    }

    private void UpdateAiming()
    {
        if (mainCamera == null || player == null || Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                Mathf.Abs(mainCamera.transform.position.z)
            )
        );

        Vector2 direction = (Vector2)(mouseWorld - player.position);

        if (direction.sqrMagnitude > 0.001f)
            aimDirection = direction.normalized;

        if (aimLine == null)
            return;

        Vector3 start = player.position;
        Vector3 end = start + (Vector3)(aimDirection * lineLength);

        aimLine.SetPosition(0, start);
        aimLine.SetPosition(1, end);
    }

    private void ThrowChestnut()
    {
        isAiming = false;

        if (aimLine != null)
            aimLine.enabled = false;

        if (selectedItemPanel == null || player == null)
            return;

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

        // Pastikan tag Throwable terpasang pada root.
        chestnut.tag = "Throwable";

        // Pastikan Rigidbody2D ada.
        Rigidbody2D rb = chestnut.GetComponent<Rigidbody2D>();

        if (rb == null)
            rb = chestnut.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = chestnutGravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.angularVelocity = 0f;

        // Pastikan Chestnut memiliki Collider2D.
        Collider2D[] chestnutColliders =
            chestnut.GetComponentsInChildren<Collider2D>(true);

        if (chestnutColliders.Length == 0)
        {
            CircleCollider2D newCollider = chestnut.AddComponent<CircleCollider2D>();
            newCollider.radius = 0.2f;

            chestnutColliders = chestnut.GetComponentsInChildren<Collider2D>(true);
        }

        // Jangan sampai Chestnut langsung menabrak Player saat spawn.
        Collider2D[] playerColliders =
            player.GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D playerCollider in playerColliders)
        {
            foreach (Collider2D chestnutCollider in chestnutColliders)
            {
                if (playerCollider != null && chestnutCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, chestnutCollider, true);
                }
            }
        }

        // Reset velocity, kemudian lempar.
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(aimDirection * throwForce, ForceMode2D.Impulse);

        Debug.Log("Chestnut berhasil dilempar!");
        Debug.Log("Tag Chestnut: " + chestnut.tag);
        Debug.Log("Arah lempar: " + aimDirection);

        // Kurangi item inventory.
        slot.ConsumeOne();

        if (slot.itemQuantity <= 0)
        {
            selectedItemPanel.ClearSelectedItem();
        }
        else
        {
            selectedItemPanel.ShowSelectedItem(slot);
        }
    }
}