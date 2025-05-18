using UnityEngine;
using System.Collections;


public class DoorInteract : MonoBehaviour
{
    public int cost = 1000;
    private PlayerPoints playerPoints;
    private UIManager uiManager;
    private bool playerInRange = false;
    private bool isOpening = false;

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

            string priceText = DoubleOrNothin.Instance != null
                ? DoubleOrNothin.Instance.FormatCostText(cost)
                : cost.ToString();

            uiManager.ShowPrompt($"Press E to Open Door - {priceText}");
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
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isOpening)
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
        Debug.Log("Door opening animation triggered.");
        isOpening = true;
        uiManager.HidePrompt();

        StartCoroutine(DoorWiggleThenFly());
    }

    private IEnumerator DoorWiggleThenFly()
    {
        Vector3 originalPosition = transform.position;
        float wiggleDuration = 1f;
        float wiggleSpeed = 20f;
        float wiggleAmount = 10f;

        float elapsed = 0f;
        while (elapsed < wiggleDuration)
        {
            float rotationOffset = Mathf.Sin(elapsed * wiggleSpeed) * wiggleAmount;
            transform.localRotation = Quaternion.Euler(rotationOffset, 0f, 0f);


            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;

        float liftSpeed = 10f;
        float liftDuration = 1f;
        float liftElapsed = 0f;

        while (liftElapsed < liftDuration)
        {
            transform.position += Vector3.up * liftSpeed * Time.deltaTime;
            liftElapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
