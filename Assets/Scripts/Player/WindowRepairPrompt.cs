using UnityEngine;

public class WindowRepairPrompt : MonoBehaviour
{
    public BarrierWindow window;          // assign in Inspector
    public KeyCode buildKey = KeyCode.F;  // hold‑to‑build
    private bool inTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) inTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) inTrigger = false;
    }

    void Update()
    {
        if (inTrigger && Input.GetKey(buildKey))
        {
            window.PlayerStartRebuild();
            //award points here per plank
        }
    }
}
