using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float delay = 2f;
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public float damage = 100f;

    [Header("FX")]
    public GameObject explosionEffectPrefab;

    private bool hasExploded = false;

    void Start()
    {
        Invoke(nameof(Explode), delay);
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Spawn VFX
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Find all colliders in range
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in hits)
        {
            // Apply force
            Rigidbody rb = nearby.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Apply damage
            EnemyHealth enemy = nearby.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, FindObjectOfType<PlayerPoints>()); // or null if no points
            }
        }

        // Clean up
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
       
        // Explode();
    }
}
