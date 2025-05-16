using UnityEngine;
using System.Collections;

public class DoubleOrNothin : MonoBehaviour
{
    public static DoubleOrNothin Instance;

    public enum PriceModifier
    {
        Normal,
        Free,
        Double
    }

    public PriceModifier CurrentModifier { get; private set; } = PriceModifier.Normal;
    public float effectDuration = 30f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TryActivateEffect();
        }
    }

    public void TryActivateEffect()
    {
        if (CurrentModifier != PriceModifier.Normal) return;

        int roll = Random.Range(0, 2);
        CurrentModifier = (roll == 0) ? PriceModifier.Free : PriceModifier.Double;
        Debug.Log($"DoubleOrNothin Effect: {CurrentModifier}");

        StartCoroutine(ResetEffectAfterTime());
    }

    private IEnumerator ResetEffectAfterTime()
    {
        yield return new WaitForSeconds(effectDuration);
        CurrentModifier = PriceModifier.Normal;
        Debug.Log("DoubleOrNothin Effect ended.");
    }

    public int GetModifiedCost(int baseCost)
    {
        return CurrentModifier switch
        {
            PriceModifier.Free => 0,
            PriceModifier.Double => baseCost * 2,
            _ => baseCost
        };
    }

    public string FormatCostText(int baseCost)
    {
        int modified = GetModifiedCost(baseCost);
        if (CurrentModifier == PriceModifier.Free)
            return "FREE";
        if (CurrentModifier == PriceModifier.Double)
            return $"DOUBLE ({modified})";
        return modified.ToString();
    }
}
