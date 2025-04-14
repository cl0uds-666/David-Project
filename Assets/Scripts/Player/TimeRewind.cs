using UnityEngine;
using System.Collections.Generic;

public class TimeRewind : MonoBehaviour
{
    public float rewindDuration = 5f; // how many seconds back we can rewind
    public KeyCode rewindKey = KeyCode.Q;

    private bool isRewinding = false;

    private List<Vector3> positions = new List<Vector3>();
    private List<Quaternion> rotations = new List<Quaternion>();
    private List<float> healths = new List<float>();

    private PlayerHealth playerHealth;
    private Rigidbody rb;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(rewindKey))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(rewindKey))
        {
            StopRewind();
        }
    }

    void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    void Record()
    {
        positions.Insert(0, transform.position);
        rotations.Insert(0, transform.rotation);
        healths.Insert(0, playerHealth.GetCurrentHealth()); // Add a getter if private

        float maxFrames = rewindDuration / Time.fixedDeltaTime;

        if (positions.Count > maxFrames)
        {
            positions.RemoveAt(positions.Count - 1);
            rotations.RemoveAt(rotations.Count - 1);
            healths.RemoveAt(healths.Count - 1);
        }
    }

    void Rewind()
    {
        if (positions.Count > 0)
        {
            transform.position = positions[0];
            transform.rotation = rotations[0];
            playerHealth.SetHealth(healths[0]); // Add setter if private

            positions.RemoveAt(0);
            rotations.RemoveAt(0);
            healths.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }

    void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true; // Prevent physics chaos
    }

    void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
    }
}
