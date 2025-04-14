using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public int cost = 1000;
    private PlayerPoints playerPoints;
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

            uiManager.ShowPrompt($"Press E to Open Door - {cost}");
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
            TryOpenDoor();
        }
    }

    private void TryOpenDoor()
    {
        if (playerPoints != null && playerPoints.SpendPoints(cost))
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("Not enough points!");
        }
    }

    private void OpenDoor()
    {
        Debug.Log("Door opened.");

        uiManager.HidePrompt(); // FORCED prompt clear always

        gameObject.SetActive(false);
    }
}
