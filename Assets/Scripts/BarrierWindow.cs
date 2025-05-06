using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarrierWindow : MonoBehaviour
{
    [Header("Planks (top‑to‑bottom order)")]
    public List<Transform> planks = new List<Transform>();

    [Header("Timings (seconds per plank)")]
    public float zombieRemoveTime = 1.2f;
    public float playerBuildTime = 0.6f;

    [Header("Offset when removed")]
    public Vector3 removedOffset = new Vector3(0f, -0.6f, 0.2f);

    // cached original positions
    private List<Vector3> originalLocalPos = new List<Vector3>();

    private bool zombieWorking = false;
    private bool playerBuilding = false;

    // ------------------------------------------------------------------
    // initialise original positions
    // ------------------------------------------------------------------
    void Awake()
    {
        foreach (Transform p in planks)
            originalLocalPos.Add(p.localPosition);
    }

    // ------------------------------------------------------------------
    // public helpers called from other scripts
    // ------------------------------------------------------------------
    public bool HasAnyPlanks()
    {
        return planks.Exists(p => p.gameObject.activeSelf);
    }

    public void ZombieStartRemoving()
    {
        if (!zombieWorking && HasAnyPlanks())
            StartCoroutine(ZombieRemoveRoutine());
    }

    public void PlayerStartRebuild()
    {
        if (!playerBuilding && planks.Exists(p => !p.gameObject.activeSelf))
            StartCoroutine(PlayerRebuildRoutine());
    }

    // ------------------------------------------------------------------
    // zombie tearing coroutine
    // ------------------------------------------------------------------
    IEnumerator ZombieRemoveRoutine()
    {
        zombieWorking = true;

        for (int i = 0; i < planks.Count; i++)
        {
            Transform plank = planks[i];
            if (!plank.gameObject.activeSelf) continue;

            Vector3 start = originalLocalPos[i];
            Vector3 target = start + removedOffset;

            float t = 0f;
            while (t < zombieRemoveTime)
            {
                t += Time.deltaTime;
                plank.localPosition = Vector3.Lerp(start, target, t / zombieRemoveTime);
                yield return null;
            }

            plank.localPosition = target;
            plank.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        zombieWorking = false;
    }

    // ------------------------------------------------------------------
    // player rebuild coroutine
    // ------------------------------------------------------------------
    IEnumerator PlayerRebuildRoutine()
    {
        playerBuilding = true;

        for (int i = planks.Count - 1; i >= 0; i--)
        {
            Transform plank = planks[i];
            if (plank.gameObject.activeSelf) continue;

            Vector3 endPos = originalLocalPos[i];
            Vector3 startPos = endPos + removedOffset;

            plank.localPosition = startPos;
            plank.gameObject.SetActive(true);

            float t = 0f;
            while (t < playerBuildTime)
            {
                t += Time.deltaTime;
                plank.localPosition = Vector3.Lerp(startPos, endPos, t / playerBuildTime);
                yield return null;
            }

            plank.localPosition = endPos;
            yield return new WaitForSeconds(0.05f);
        }

        playerBuilding = false;
    }
}
