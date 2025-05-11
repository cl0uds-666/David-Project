using UnityEngine;

public class MysteryBoxClosed : MonoBehaviour
{
    public GameObject openBoxPrefab;
    public int cost = 950;

    private PlayerPoints playerPoints;
    private bool isActive = false;

    void Start()
    {
        playerPoints = FindObjectOfType<PlayerPoints>();
    }

    void OnTriggerStay(Collider other)
    {
        if (isActive) return;

        if (Input.GetKeyDown(KeyCode.E) && playerPoints != null && playerPoints.SpendPoints(cost))
        {
            isActive = true;

            openBoxPrefab.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
