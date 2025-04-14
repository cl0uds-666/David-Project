using UnityEngine;
using System.Collections.Generic;

public class RewindableObject : MonoBehaviour, IRewindable
{
    public float rewindDuration = 5f;

    private List<Vector3> positions = new List<Vector3>();
    private List<Quaternion> rotations = new List<Quaternion>();
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Record()
    {
        if (positions.Count == 0 || transform.position != positions[0])
        {
            positions.Insert(0, transform.position);
            rotations.Insert(0, transform.rotation);

            int maxFrames = Mathf.RoundToInt(rewindDuration / Time.fixedDeltaTime);
            if (positions.Count > maxFrames)
            {
                positions.RemoveAt(positions.Count - 1);
                rotations.RemoveAt(rotations.Count - 1);
            }
        }
    }

    public void Rewind()
    {
        if (positions.Count > 0)
        {
            transform.position = positions[0];
            transform.rotation = rotations[0];

            int rewindSpeed = 3; // Skip frames to speed it up
            int removeCount = Mathf.Min(rewindSpeed, positions.Count);
            positions.RemoveRange(0, removeCount);
            rotations.RemoveRange(0, removeCount);
        }
    }
}
