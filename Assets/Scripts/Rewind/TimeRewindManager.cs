using UnityEngine;
using System.Linq;

public class TimeRewindManager : MonoBehaviour
{
    private IRewindable[] rewindables;
    public KeyCode rewindKey = KeyCode.Q;
    private bool isRewinding = false;

    private void Start()
    {
        rewindables = FindObjectsOfType<MonoBehaviour>(true).OfType<IRewindable>().ToArray();
    }

    private void Update()
    {
        // Find all objects implementing IRewindable (Spawning Zombies)
        rewindables = FindObjectsOfType<MonoBehaviour>(true).OfType<IRewindable>().ToArray();


        if (Input.GetKeyDown(rewindKey))
        {
            StartRewind();
        }

        if (Input.GetKeyUp(rewindKey))
        {
            StopRewind();
        }
    }

    private void FixedUpdate()
    {
        foreach (var rewindable in rewindables)
        {
            if (isRewinding)
            {
                rewindable.Rewind();
            }
            else
            {
                rewindable.Record();
            }
        }
    }

    private void StartRewind()
    {
        isRewinding = true;
        Debug.Log("Rewinding World...");
    }

    private void StopRewind()
    {
        isRewinding = false;
        Debug.Log("Stopped Rewind.");
    }
}
