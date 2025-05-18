using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI roundText; // Add this for round display

    [Header("Round Flash Settings")]
    public float flashDuration = 0.5f;
    public Vector3 flashScale = new Vector3(1.5f, 1.5f, 1f);
    public Color flashColor = Color.red;

    private Color originalRoundColor;

    void Start()
    {
        if (roundText != null)
            originalRoundColor = roundText.color;
    }

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

    public void SetRound(int round)
    {
        if (roundText == null) return;

        roundText.text = "Round " + round;
        StartCoroutine(FlashRoundUI());
    }

    private IEnumerator FlashRoundUI()
    {
        float timer = 0f;
        roundText.color = flashColor;

        while (timer < flashDuration)
        {
            float t = timer / flashDuration;
            float scale = Mathf.Lerp(1f, flashScale.x, Mathf.PingPong(t * 2f, 1f));
            roundText.transform.localScale = new Vector3(scale, scale, 1f);

            timer += Time.deltaTime;
            yield return null;
        }

        roundText.transform.localScale = Vector3.one;
        roundText.color = originalRoundColor;
    }

    public IEnumerator PulseUntilRoundStart(float duration, int upcomingRound)
    {
        float timer = 0f;
        roundText.color = flashColor;

        while (timer < duration)
        {
            float t = Mathf.PingPong(timer * 2f, 1f);
            float scale = Mathf.Lerp(1f, flashScale.x, t);
            roundText.transform.localScale = new Vector3(scale, scale, 1f);

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset scale and color
        roundText.transform.localScale = Vector3.one;
        roundText.color = originalRoundColor;

        // Update to new round and flash once
        SetRound(upcomingRound);
    }


}
