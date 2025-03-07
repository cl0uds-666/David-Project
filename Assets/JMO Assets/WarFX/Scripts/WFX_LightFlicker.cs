using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{
    public float time = 0.05f;
    private float timer;
    private bool isFlickering = false;
    private Light lightSource;

    void Start()
    {
        lightSource = GetComponent<Light>();
        lightSource.enabled = false; // Ensure light is off at start
    }

    public void StartFlicker()
    {
        if (!isFlickering)
        {
            StartCoroutine(Flicker());
        }
    }

    IEnumerator Flicker()
    {
        isFlickering = true;
        lightSource.enabled = true; // Turn on light when shooting
        yield return new WaitForSeconds(time);
        lightSource.enabled = false; // Turn off light after flicker time
        isFlickering = false;
    }
}
