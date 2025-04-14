using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI promptText;

    public void ShowPrompt(string message)
    {
        promptText.text = message;
        promptText.enabled = true;
    }

    public void HidePrompt()
    {
        promptText.text = "";
        promptText.enabled = false;
    }
}
