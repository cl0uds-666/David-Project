using UnityEngine;
using TMPro;

public class MysteryBoxClosed : MonoBehaviour
{
    public GameObject openBox; // Assign the MysteryBoxOpen object here (not a prefab)
    public int cost = 950;

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText;

    private PlayerPoints playerPoints;
    private bool isActive = false;
    private bool playerInRange = false;

    void Start()
    {
        playerPoints = FindObjectOfType<PlayerPoints>();

        if (promptText != null)
        {
            promptText.enabled = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isActive || !other.CompareTag("Player")) return;

        if (promptText != null && !promptText.enabled)
        {
            string costText = DoubleOrNothin.Instance != null
                ? DoubleOrNothin.Instance.FormatCostText(cost)
                : cost.ToString();

            promptText.text = $"Press [E] to roll the box ({costText})";
            promptText.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && playerPoints != null && playerPoints.SpendPoints(cost))
        {
            isActive = true;

            if (promptText != null)
                promptText.enabled = false;

            if (openBox != null)
            {
                openBox.SetActive(true);

                MysteryBoxOpen openScript = openBox.GetComponent<MysteryBoxOpen>();
                if (openScript != null)
                {
                    openScript.BeginRoll();
                }
                else
                {
                    Debug.LogWarning("MysteryBoxClosed: openBox is missing MysteryBoxOpen script!");
                }
            }

            gameObject.SetActive(false); // Hide closed box
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (promptText != null)
            promptText.enabled = false;
    }

    public void ReactivateBox()
    {
        isActive = false;

        if (promptText != null)
            promptText.enabled = false;

        gameObject.SetActive(true);
    }
}
