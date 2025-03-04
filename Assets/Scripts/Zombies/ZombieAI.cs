using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    public float randomOffsetRange = 2f; // Range for offset movement

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find the player automatically
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (target != null && agent != null)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-randomOffsetRange, randomOffsetRange),
                0,
                Random.Range(-randomOffsetRange, randomOffsetRange)
            );

            Vector3 newTargetPosition = target.position + randomOffset;
            agent.SetDestination(newTargetPosition);
        }
    }
}
