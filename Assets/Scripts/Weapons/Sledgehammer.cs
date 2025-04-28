using UnityEngine;
using System.Collections.Generic;

public class Sledgehammer : MonoBehaviour
{
    [Header("Settings")]
    public float throwSpeed = 20f;
    public float spinSpeed = 720f;
    public float targetRadius = 30f; // Distance to find zombies
    public int maxTargets = 5;

    [Header("References")]
    public Transform hammerModel;
    public Transform player;
    public LayerMask zombieLayer;

    private bool isThrown = false;
    private bool isReturning = false;
    private Queue<Transform> targets = new Queue<Transform>();
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (isThrown)
        {
            if (hammerModel != null)
            {
                hammerModel.Rotate(Vector3.right, spinSpeed * Time.deltaTime);
            }

            if (targets.Count > 0)
            {
                Transform nextTarget = targets.Peek();

                // Check if the target still exists
                if (nextTarget == null)
                {
                    targets.Dequeue();
                    return;
                }

                Vector3 direction = (nextTarget.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, nextTarget.position, throwSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, nextTarget.position) < 0.5f)
                {
                    HitZombie(nextTarget);
                    targets.Dequeue();
                }
            }
            else
            {
                // No targets left, start returning
                isThrown = false;
                isReturning = true;
            }
        }
        else if (isReturning)
        {
            if (hammerModel != null)
            {
                hammerModel.Rotate(Vector3.right, spinSpeed * Time.deltaTime);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, throwSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, spinSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.localPosition, originalPosition) < 0.05f)
            {
                isReturning = false;
                transform.localPosition = originalPosition;
                transform.localRotation = originalRotation;
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                StartThrow();
            }
        }
    }

    void StartThrow()
    {
        isThrown = true;

        if (hammerModel != null)
        {
            hammerModel.localRotation = Quaternion.identity;
        }

        Collider[] hits = Physics.OverlapSphere(player.position, targetRadius, zombieLayer);

        List<Transform> nearbyZombies = new List<Transform>();
        foreach (Collider hit in hits)
        {
            EnemyHealth health = hit.GetComponent<EnemyHealth>();
            if (health != null && health.currentHealth > 0f)
            {
                nearbyZombies.Add(hit.transform);
            }
        }

        // Sort by distance
        nearbyZombies.Sort((a, b) => Vector3.Distance(player.position, a.position).CompareTo(Vector3.Distance(player.position, b.position)));

        for (int i = 0; i < Mathf.Min(maxTargets, nearbyZombies.Count); i++)
        {
            targets.Enqueue(nearbyZombies[i]);
        }
    }


    void HitZombie(Transform zombie)
    {
        if (zombie == null) return;

        EnemyHealth health = zombie.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(99999f, FindObjectOfType<PlayerPoints>());
        }
    }
}
