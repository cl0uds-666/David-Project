using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int pointsPerHit = 10;
    public int pointsOnKill = 50;

    public event Action onDeath;
    private bool isDead = false;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;

    [Header("Hitbox Collider (Used for Damage Detection)")]
    public Collider hitboxCollider;

    void Start()
    {
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

    public void TakeDamage(float amount, PlayerPoints playerPoints)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage. Current HP: " + currentHealth);

        if (playerPoints != null)
        {
            playerPoints.AddPoints(pointsPerHit);
        }

        if (currentHealth <= 0)
        {
            Die(playerPoints);
        }
    }

    void Die(PlayerPoints playerPoints)
    {
        if (isDead) return;
        isDead = true;

        onDeath?.Invoke();

        if (agent != null) agent.enabled = false;
        if (animator != null) animator.enabled = false;

        SetRagdollState(true);

        if (playerPoints != null)
        {
            playerPoints.AddPoints(pointsOnKill);
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
