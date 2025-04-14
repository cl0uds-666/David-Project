using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public string message;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && promptText != null)
        {
            promptText.text = message;
            promptText.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && promptText != null)
        {
            promptText.enabled = false;
        }
    }
}
