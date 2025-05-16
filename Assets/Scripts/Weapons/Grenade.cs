using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;

    [Header("Damage Values")]
    public float zombieDamage = 500f;
    public float playerDamage = 50f;

    public GameObject explosionEffect;

    private float countdown;
    private bool hasExploded = false;

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        // Spawn explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Damage enemies
            EnemyHealth enemy = nearby.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(zombieDamage, FindObjectOfType<PlayerPoints>());
            }

            // Damage player (unless immune)
            PlayerHealth player = nearby.GetComponent<PlayerHealth>();
            if (player != null && !player.isExplosionImmune)
            {
                player.TakeDamage(playerDamage);
            }
        }

        Destroy(gameObject);
    }
}
