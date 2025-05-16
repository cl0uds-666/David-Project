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

    [Header("Zombie Sounds")]
    public AudioSource audioSource;
    public AudioClip[] zombieSounds;
    public float minSoundInterval = 3f;
    public float maxSoundInterval = 8f;
    private float nextSoundTime;

    [Header("Speed Scaling")]
    public float baseSpeed = 1.5f;
    public float speedIncreasePerRound = 0.15f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }

        // Set first random sound time
        nextSoundTime = Time.time + Random.Range(minSoundInterval, maxSoundInterval);

        // Get round number
        int round = FindObjectOfType<ZombieSpawner>()?.GetCurrentRound() ?? 1;

        float finalSpeed = baseSpeed;

        // Determine if this zombie should sprint
        bool shouldSprint = false;

        if (round >= 5 && round < 15)
        {
            float sprintChance = Mathf.InverseLerp(5f, 15f, round) * 100f;
            shouldSprint = Random.Range(0f, 100f) <= sprintChance;
            if (shouldSprint)
                finalSpeed = 6f;
        }
        else if (round >= 15 && round < 20)
        {
            shouldSprint = true;
            finalSpeed = 6f;
        }
        else if (round >= 20)
        {
            shouldSprint = true;
            finalSpeed = 9f;
        }
        else
        {
            shouldSprint = false;
            finalSpeed = baseSpeed + (round - 1) * speedIncreasePerRound;
        }

        agent.speed = finalSpeed;
    }


    void Update()
    {
        if (isDead || agent == null || !agent.enabled) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.6f, transform.forward, out hit, 1.2f))
        {
            BarrierWindow window = hit.transform.GetComponentInParent<BarrierWindow>();
            if (window != null && window.HasAnyPlanks())
            {
                agent.isStopped = true;
                window.ZombieStartRemoving();
                return;
            }
        }
        else
        {
            agent.isStopped = false;
        }

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

                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(newTargetPosition);
                }
            }

            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                TryAttack();
            }
        }

        PlayRandomZombieSound();
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
                playerHealth.TakeDamage(50f);
            }
        }
    }

    void PlayRandomZombieSound()
    {
        if (Time.time >= nextSoundTime && zombieSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomClip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.PlayOneShot(randomClip);
            nextSoundTime = Time.time + Random.Range(minSoundInterval, maxSoundInterval);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null && agent.enabled)
        {
            agent.enabled = false;
        }

        this.enabled = false;
    }
}
