using UnityEngine;

public class WallBuy : MonoBehaviour
{
    public int weaponCost = 1000;
    public int ammoCost = 500;

    [Header("Weapon Settings")]
    public GameObject weaponObject;  // Drag weapon from Player's WeaponHolder
    public string weaponName = "Pistol";

    private PlayerPoints playerPoints;
    private WeaponManager weaponManager;
    private UIManager uiManager;
    private bool playerInRange = false;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerPoints = other.GetComponent<PlayerPoints>();
            weaponManager = other.GetComponent<WeaponManager>();

            if (DoubleOrNothin.Instance == null)
            {
                Debug.LogWarning("DoubleOrNothin instance not found!");
            }

            if (weaponManager != null && weaponManager.HasWeapon(weaponObject))
            {
                int displayCost = DoubleOrNothin.Instance != null
                    ? DoubleOrNothin.Instance.GetModifiedCost(ammoCost)
                    : ammoCost;

                string costText = DoubleOrNothin.Instance != null
                    ? DoubleOrNothin.Instance.FormatCostText(ammoCost)
                    : ammoCost.ToString();

                uiManager.ShowPrompt($"Press E to Refill Ammo - {costText}");
            }
            else
            {
                int displayCost = DoubleOrNothin.Instance != null
                    ? DoubleOrNothin.Instance.GetModifiedCost(weaponCost)
                    : weaponCost;

                string costText = DoubleOrNothin.Instance != null
                    ? DoubleOrNothin.Instance.FormatCostText(weaponCost)
                    : weaponCost.ToString();

                uiManager.ShowPrompt($"Press E to Buy {weaponName} - {costText}");
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerPoints = null;
            weaponManager = null;

            uiManager.HidePrompt();
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryBuy();
        }
    }

    private void TryBuy()
    {
        if (playerPoints == null || weaponManager == null)
            return;

        if (weaponManager.HasWeapon(weaponObject))
        {
            // Refill ammo
            if (playerPoints.SpendPoints(ammoCost))
            {
                Debug.Log($"Refilled ammo for {weaponName}");
                // Later: Call your weapon's Reload/AmmoReset function here
                uiManager.HidePrompt();
            }
            else
            {
                Debug.Log("Not enough points for ammo refill.");
            }
        }
        else
        {
            // Buy weapon
            if (playerPoints.SpendPoints(weaponCost))
            {
                weaponManager.AddWeapon(weaponObject);
                Debug.Log($"Bought {weaponName}");
                uiManager.HidePrompt();
            }
            else
            {
                Debug.Log("Not enough points to buy weapon.");
            }
        }
    }
}
