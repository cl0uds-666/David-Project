using UnityEngine;
using System.Linq;

public class TimeRewindManager : MonoBehaviour
{
    private IRewindable[] rewindables;
    public KeyCode rewindKey = KeyCode.Q;
    private bool isRewinding = false;

    private void Update()
    {
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
        if (rewindables == null) return;

        foreach (var rewindable in rewindables)
        {
            if (rewindable == null) continue; // Skip destroyed/null objects

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

        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null) player.isInvincible = true;
    }

    private void StopRewind()
    {
        isRewinding = false;
        Debug.Log("Stopped Rewind.");

        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null) player.isInvincible = false;
    }
}
