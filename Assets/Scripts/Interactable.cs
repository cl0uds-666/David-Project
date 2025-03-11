using UnityEngine;

public class Interactable : MonoBehaviour
{
    public int cost = 500; // Variable cost per object
    private PlayerPoints playerPoints;

    void Start()
    {
        playerPoints = FindObjectOfType<PlayerPoints>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) // 'E' to interact
        {
            if (playerPoints.SpendPoints(cost))
            {
                Unlock();
            }
            else
            {
                Debug.Log("Not enough points!");
            }
        }
    }

    void Unlock()
    {
        if (gameObject.CompareTag("Door"))
        {
            gameObject.SetActive(false); //Disabling door
        }
    }    
}
