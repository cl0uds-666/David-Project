using UnityEngine;

public class PerkMachine : MonoBehaviour
{
    public string perkName = "Time Loop Tonic";
    public int perkCost = 1500;

    private bool playerInRange = false;
    private bool perkPurchased = false;

    private PlayerPoints playerPoints;
    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !perkPurchased)
        {
            playerInRange = true;
            playerPoints = other.GetComponent<PlayerPoints>();

            uiManager.ShowPrompt($"Press E to buy {perkName} - {perkCost}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerPoints = null;

            uiManager.HidePrompt();
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryBuyPerk();
        }
    }

    private void TryBuyPerk()
    {
        if (perkPurchased)
            return;

        if (playerPoints != null && playerPoints.SpendPoints(perkCost))
        {
            perkPurchased = true;
            Debug.Log($"Bought perk: {perkName}");

            uiManager.HidePrompt(); // Hide text when bought

            // Future: ApplyPerk(perkName); goes here
        }
        else
        {
            Debug.Log("Not enough points.");
        }
    }
}
