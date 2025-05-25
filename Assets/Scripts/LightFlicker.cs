using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light lightSource;

    [Header("Flicker Settings")]
    public float minFlickerDelay = 3f;
    public float maxFlickerDelay = 8f;
    public float flickerDuration = 0.2f;
    public int flickerCount = 3;

    void Start()
    {
        lightSource = GetComponentInChildren<Light>();

        if (lightSource != null)
            StartCoroutine(FlickerRoutine());
        else
            Debug.LogWarning("LightFlicker: No Light component found in children!");
    }

    System.Collections.IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minFlickerDelay, maxFlickerDelay);
            yield return new WaitForSeconds(waitTime);

            for (int i = 0; i < flickerCount; i++)
            {
                lightSource.enabled = false;
                yield return new WaitForSeconds(Random.Range(0.02f, flickerDuration / 2));
                lightSource.enabled = true;
                yield return new WaitForSeconds(Random.Range(0.02f, flickerDuration / 2));
            }
        }
    }
}
