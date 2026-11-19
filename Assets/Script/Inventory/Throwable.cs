using UnityEngine;

public class Throwable : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float destroyAfter = 10f;

    [Header("Destroy Effect")]
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private float effectDuration = 2f;

    private bool hasHit;

    private void Start()
    {
        Invoke(nameof(TimeoutDestroy), destroyAfter);
    }

    private void TimeoutDestroy()
    {
        DestroyThrowable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;

        DestroyThrowable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        DestroyThrowable();
    }

    public void DestroyThrowable()
    {
        if (hasHit)
            return;

        hasHit = true;
        CancelInvoke();

        // Spawn effect SEBELUM Chestnut dihancurkan
        if (destroyEffect != null)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);

            // Pastikan semua particle system di dalam prefab dimainkan
            ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>(true);

            foreach (ParticleSystem particle in particles)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Play(true);
            }

            // Efek hidup sendiri, tidak ikut hilang bersama Chestnut
            effect.transform.SetParent(null);
            Destroy(effect, effectDuration);

            Debug.Log("Chestnut effect spawned: " + effect.name);
        }
        else
        {
            Debug.LogWarning("Destroy Effect belum di-assign di Throwable!");
        }

        Destroy(gameObject);
    }
}