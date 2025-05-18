using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int pointsPerHit = 10;
    public int pointsOnKill = 50;




    public float lifeTime = 60f;

    public event Action<EnemyHealth, bool> onDeath;
    private bool expiredByTimer = false;
    private bool isDead = false;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;

    private float spawnTime;

    [Header("Hit FX")]
    public GameObject hitMarkerPrefab;
    public GameObject bloodFXPrefab;


    [Header("Hitbox Collider (Used for Damage Detection)")]
    public Collider hitboxCollider;

    void Start()
    {
        spawnTime = Time.time;

        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Get all Rigidbodies & Colliders for ragdoll
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Set up ragdoll
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.mass = 1f;
            rb.drag = 0.1f;
        }

        SetRagdollState(false);

        // Dynamic Health based on round
        int currentRound = FindObjectOfType<ZombieSpawner>()?.GetCurrentRound() ?? 1;
        currentHealth = CalculateHealthForRound(currentRound);
    }

    void Update()                     
    {
        if (!isDead && Time.time - spawnTime >= lifeTime)
        {
            ForceExpire();
        }
    }

    void ForceExpire()
    {
        if (isDead) return;
        expiredByTimer = true;
        TakeDamage(currentHealth + 1f, null);
    }


    public void TakeDamage(float amount, PlayerPoints playerPoints)
    {
        if (isDead) return;

        if (PowerupEffects.Instance != null && PowerupEffects.Instance.IsInstaKill())
        {
            amount = currentHealth + 1f;
            Debug.Log("Insta-Kill applied to: " + gameObject.name);
        }


        currentHealth -= amount;

        if (playerPoints != null)
        {
            int hitPoints = PowerupEffects.Instance != null && PowerupEffects.Instance.IsDoublePoints()
                ? pointsPerHit * 2
                : pointsPerHit;

            playerPoints.AddPoints(hitPoints);
        }


        // --- Spawn floating hit marker ---
        if (hitMarkerPrefab != null)
            if (hitMarkerPrefab != null)
            {
                int hitPoints = PowerupEffects.Instance != null && PowerupEffects.Instance.IsDoublePoints()
                    ? pointsPerHit * 2
                    : pointsPerHit;

                GameObject marker = Instantiate(hitMarkerPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
                HitMarkerWorld hm = marker.GetComponent<HitMarkerWorld>();
                if (hm != null)
                    hm.SetPoints(hitPoints);
            }

        // --- Spawn blood FX ---
        if (bloodFXPrefab != null)
        {
            GameObject blood = Instantiate(bloodFXPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity);
            ParticleSystem ps = blood.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

            if (currentHealth <= 0f)
            Die(playerPoints);
    }


    void Die(PlayerPoints playerPoints)
    {
        if (isDead) return;
        isDead = true;

        bool countAsKill = !expiredByTimer;
        onDeath?.Invoke(this, countAsKill);


        if (agent != null) agent.enabled = false;
        if (animator != null) animator.enabled = false;

        SetRagdollState(true);
        PowerupSpawner spawner = FindObjectOfType<PowerupSpawner>();
        if (spawner != null)
        {
            spawner.TrySpawnPowerup(transform.position);
        }


        if (playerPoints != null)
        {
            int killPoints = PowerupEffects.Instance != null && PowerupEffects.Instance.IsDoublePoints()
                ? pointsOnKill * 2
                : pointsOnKill;

            playerPoints.AddPoints(killPoints);

            // Show floating marker for kill points
            if (hitMarkerPrefab != null)
            {
                GameObject marker = Instantiate(hitMarkerPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
                HitMarkerWorld hm = marker.GetComponent<HitMarkerWorld>();
                if (hm != null)
                    hm.SetPoints(killPoints);
            }
        }



        Destroy(gameObject, 10f);
    }

    void SetRagdollState(bool state)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col != hitboxCollider)
            {
                col.enabled = state;
            }
        }
    }

    private float CalculateHealthForRound(int round)
    {
        float baseHealth = maxHealth;

        if (round <= 10)
        {
            return baseHealth + (round - 1) * 75f;
        }
        else if (round <= 20)
        {
            return baseHealth + (9 * 75f) + (round - 10) * 150f;
        }
        else if (round <= 40)
        {
            float healthAt20 = baseHealth + (9 * 75f) + (10 * 150f);
            return healthAt20 * Mathf.Pow(1.15f, round - 20);
        }
        else
        {
            return 5000f; // Hard cap at round 40
        }
    }
}
