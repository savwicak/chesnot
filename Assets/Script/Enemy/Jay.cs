using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class Jay : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int hp = 100;

    [Header("Movement")]
    [SerializeField] private float flySpeed = 4f;
    [SerializeField] private float jarakDeteksi = 5f;
    [SerializeField] private float jarakKabur = 8f;

    [Header("References")]
    [SerializeField] private Collider2D jayCollider;
    [SerializeField] private Animator jayAnimator;

    [Header("Animation Names")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string flyAnimation = "Fly";

    [Header("Death Event")]
    [SerializeField] private UnityEvent onJayDeadVisual;

    protected Transform player;

    private StateJay state = StateJay.IDLE;
    private bool isDead;

    private readonly List<Collider2D> detectedColliders = new List<Collider2D>();

    public static event Action<Jay> OnJayMati;

    private enum StateJay
    {
        IDLE,
        CHASE
    }

    private void Awake()
    {
        if (jayCollider == null)
            jayCollider = GetComponent<Collider2D>();

        if (jayAnimator == null)
            jayAnimator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        state = StateJay.IDLE;

        if (jayAnimator != null)
            jayAnimator.Play(idleAnimation);

        Debug.Log("Jay mulai Idle.");
    }

    private void Update()
    {
        if (isDead)
            return;

        CekThrowableOverlap();

        if (isDead || player == null)
            return;

        PeriksaTransisi();

        switch (state)
        {
            case StateJay.IDLE:
                PerilakuIdle();
                break;

            case StateJay.CHASE:
                PerilakuChase();
                break;
        }
    }

    private void PeriksaTransisi()
    {
        float jarak = JarakKePlayer();

        if (state == StateJay.IDLE && jarak <= jarakDeteksi)
        {
            MulaiChase();
        }
        else if (state == StateJay.CHASE && jarak >= jarakKabur)
        {
            KembaliIdle();
        }
    }

    public float JarakKePlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        return Vector2.Distance(transform.position, player.position);
    }

    private void PerilakuIdle()
    {
        // Jay diam saat Player jauh.
    }

    private void MulaiChase()
    {
        state = StateJay.CHASE;

        if (jayAnimator != null)
            jayAnimator.Play(flyAnimation);

        Debug.Log("Jay mulai mengejar Player!");
    }

    private void KembaliIdle()
    {
        state = StateJay.IDLE;

        if (jayAnimator != null)
            jayAnimator.Play(idleAnimation);

        Debug.Log("Jay kembali Idle.");
    }

    private void PerilakuChase()
    {
        if (player == null)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            flySpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CekThrowable(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CekThrowable(collision.collider);

        if (collision.otherCollider != null)
            CekThrowable(collision.otherCollider);
    }

    private void CekThrowableOverlap()
    {
        if (jayCollider == null || isDead)
            return;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(Physics2D.AllLayers);

        detectedColliders.Clear();

        jayCollider.Overlap(filter, detectedColliders);

        foreach (Collider2D col in detectedColliders)
        {
            if (col == null || col == jayCollider)
                continue;

            CekThrowable(col);

            if (isDead)
                return;
        }
    }

    private void CekThrowable(Collider2D other)
    {
        if (isDead || other == null)
            return;

        Transform throwableTransform = CariThrowable(other.transform);

        if (throwableTransform == null)
            return;

        Debug.Log("Jay terkena Throwable: " + throwableTransform.name);

        Mati(throwableTransform.gameObject);
    }

    private Transform CariThrowable(Transform target)
    {
        Transform current = target;

        while (current != null)
        {
            if (current.CompareTag("Throwable"))
                return current;

            current = current.parent;
        }

        return null;
    }

    public void KenaDamage(int jumlah)
    {
        if (isDead)
            return;

        hp -= jumlah;

        Debug.Log($"{gameObject.name} terkena damage {jumlah}. HP: {hp}");

        if (hp <= 0)
            Mati();
    }

    private void Mati(GameObject throwable = null)
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Jay mati!");

        if (throwable != null)
            Destroy(throwable);

        OnJayMati?.Invoke(this);

        if (onJayDeadVisual != null)
            onJayDeadVisual.Invoke();

        Destroy(gameObject);
    }
}