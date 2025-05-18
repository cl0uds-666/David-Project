using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public string message = "Press E to interact ({COST})";
    public int baseCost = 1000;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && promptText != null)
        {
            string costText = DoubleOrNothin.Instance != null
                ? DoubleOrNothin.Instance.FormatCostText(baseCost)
                : baseCost.ToString();

            string finalMessage = message.Replace("{COST}", costText);
            promptText.text = finalMessage;
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
