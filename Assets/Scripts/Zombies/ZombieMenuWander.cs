using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieMenuWander : MonoBehaviour
{
    [Header("Way-Points")]
    public Transform[] points;
    public bool randomOrder = true;

    [Header("Behaviour")]
    public float idleTime = 1.5f;
    public float minSpeed = 0.6f;
    public float maxSpeed = 1.2f;
    public float turnSpeed = 5f;          // how fast the zombie turns toward velocity

    private NavMeshAgent agent;
    private int currentIndex = 0;
    private float idleTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;     // we’ll handle rotation manually
        PickNextDestination();
    }

    void Update()
    {
        // ------- move / idle logic -------
        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                PickNextDestination();
                idleTimer = 0f;
            }
        }

        // ------- face walking direction -------
        Vector3 horizontalVel = new Vector3(agent.velocity.x, 0f, agent.velocity.z);
        if (horizontalVel.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(horizontalVel);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }

    void PickNextDestination()
    {
        if (points.Length == 0) return;

        currentIndex = randomOrder
            ? Random.Range(0, points.Length)
            : (currentIndex + 1) % points.Length;

        agent.speed = Random.Range(minSpeed, maxSpeed);
        agent.SetDestination(points[currentIndex].position);
    }
}
