using UnityEngine;
using System.Linq;

public class TimeRewindManager : MonoBehaviour
{
    private IRewindable[] rewindables;
    public KeyCode rewindKey = KeyCode.Q;
    private bool isRewinding = false;

    private void Update()
    {
        // Get all active IRewindable components from live MonoBehaviours only
        rewindables = FindObjectsOfType<MonoBehaviour>(true)
            .Where(m => m != null && m.gameObject != null)
            .OfType<IRewindable>()
            .ToArray();

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
            if (rewindable == null) continue;

            try
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
            catch (MissingReferenceException)
            {
                // Swallow ghost reference from Unity destroying the object
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
