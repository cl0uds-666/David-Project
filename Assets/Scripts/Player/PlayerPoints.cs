using UnityEngine;
using TMPro;

public class PlayerPoints : MonoBehaviour
{
    public int points = 500; // Starting points
    public TextMeshProUGUI pointsText;

    void Start()
    {
        UpdatePointsUI();
    }

    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsUI();
    }

    public bool SpendPoints(int baseCost)
    {
        int finalCost = DoubleOrNothin.Instance != null
            ? DoubleOrNothin.Instance.GetModifiedCost(baseCost)
            : baseCost;

        if (points >= finalCost)
        {
            points -= finalCost;
            UpdatePointsUI();
            return true;
        }
        return false;
    }


    void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = points.ToString();
        }
    }
}
