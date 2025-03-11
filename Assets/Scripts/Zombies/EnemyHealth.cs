using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public int pointsPerHit = 10;
    public int pointsOnKill = 100;

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
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Get all Rigidbodies & Colliders for ragdoll
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Ensure all ragdoll rigidbodies are kinematic initially
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.mass = 1f; // Prevents excessive physics force
            rb.drag = 0.1f; // Small drag to avoid wild movement
        }

        SetRagdollState(false);
    }

    public void TakeDamage(float amount, PlayerPoints playerPoints)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage. Current HP: " + currentHealth);

        if (playerPoints != null)
        {
            playerPoints.AddPoints(pointsPerHit); // Points per hit
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

        // Disable AI movement
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Disable animations
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Enable Ragdoll physics
        SetRagdollState(true);

        if (playerPoints != null)
        {
            playerPoints.AddPoints(pointsOnKill); // Extra points on kill
        }

        // Destroy after a while (optional)
        Destroy(gameObject, 10f);
    }

    void SetRagdollState(bool state)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state; // Enable physics
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col != hitboxCollider) // Keep hitbox active
            {
                col.enabled = state;
            }
        }
    }
}
