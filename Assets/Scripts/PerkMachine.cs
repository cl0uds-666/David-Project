using UnityEngine;

public class PerkMachine : MonoBehaviour
{
    public string perkName = "Health+ Hooch";
    public int perkCost = 1500;

    private bool playerInRange = false;
    private bool perkPurchased = false;

    private PlayerPoints playerPoints;
    private UIManager uiManager;

    public GameObject perkUIImage; // Assign in inspector: UI icon that shows when perk is active

    void Start()
    {
        playerPoints = FindObjectOfType<PlayerPoints>();
        uiManager = FindObjectOfType<UIManager>();

        if (perkUIImage != null) 
            perkUIImage.SetActive(false); // Make sure it's hidden at start
    }

    void Update()
    {
        if (playerInRange && !perkPurchased && Input.GetKeyDown(KeyCode.F))
        {
            TryBuyPerk();
        }
    }

    private void TryBuyPerk()
    {
        if (perkPurchased) return;

        if (playerPoints != null && playerPoints.SpendPoints(perkCost))
        {
            perkPurchased = true;
            Debug.Log($"Bought perk: {perkName}");

            uiManager.HidePrompt();

            ApplyPerk(perkName);

            if (perkUIImage != null)
                perkUIImage.SetActive(true);
        }
        else
        {
            Debug.Log("Not enough points to buy perk.");
        }
    }

    private void ApplyPerk(string perk)
    {
        PlayerHealth health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        switch (perk)
        {
            case "Health+ Hooch":
                if (health != null)
                {
                    health.IncreaseMaxHealth(100);
                    Debug.Log("Health+ Hooch applied! Max health increased.");
                }
                break;

            case "DoubleOrNothin Daiquiri":
                if (DoubleOrNothin.Instance != null)
                {
                    // Optional: Show a message to player that it’s ready
                    Debug.Log("DoubleOrNothin Daiquiri activated. Press Y to roll the dice!");
                }
                break;

            default:
                Debug.LogWarning("Unknown perk: " + perk);
                break;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !perkPurchased)
        {
            playerInRange = true;

            if (uiManager != null)
                uiManager.ShowPrompt($"Press [F] to buy {perkName} ({perkCost})");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (uiManager != null)
                uiManager.HidePrompt();
        }
    }
}
