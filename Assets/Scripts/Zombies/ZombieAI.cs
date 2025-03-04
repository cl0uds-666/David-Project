using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private float nextPathUpdateTime = 0f;
    private float pathUpdateInterval = 0.2f;
    private bool isDead = false;

    public float randomOffsetRange = 2f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (isDead || agent == null || !agent.enabled) return; // Prevent errors after death

        if (target != null && agent.enabled)
        {
            if (Time.time >= nextPathUpdateTime)
            {
                nextPathUpdateTime = Time.time + pathUpdateInterval;

                Vector3 randomOffset = new Vector3(
                    Random.Range(-randomOffsetRange, randomOffsetRange),
                    0,
                    Random.Range(-randomOffsetRange, randomOffsetRange)
                );

                Vector3 newTargetPosition = target.position + randomOffset;

                if (agent.isOnNavMesh) // Check if the agent is on a valid NavMesh
                {
                    agent.SetDestination(newTargetPosition);
                }
            }

            // Check if the zombie is close enough to attack
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                TryAttack();
            }
        }
    }

    void TryAttack()
    {
        if (isDead) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Zombie Attacks!");

            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null && agent.enabled)
        {
            agent.enabled = false; // Disable NavMeshAgent before physics takes over
        }

        // Disable AI script to stop further updates
        this.enabled = false;
    }
}
