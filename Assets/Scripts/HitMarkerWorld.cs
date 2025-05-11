using UnityEngine;
using TMPro;

public class HitMarkerWorld : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeTime = 0.8f;
    public TextMeshPro text;

    private float timer = 0f;
    private Color originalColor;

    void Start()
    {
        // If not assigned, try to auto-find
        if (text == null)
        {
            text = GetComponentInChildren<TextMeshPro>();
        }

        if (text != null)
        {
            originalColor = text.color;
        }
        else
        {
            Debug.LogError("HitMarkerWorld: No TextMeshPro assigned or found.");
        }
    }

    void Update()
    {


        if (text == null) return;

        


        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0f, timer / fadeTime);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);

        if (timer >= fadeTime)
            Destroy(gameObject);
    }

    public void SetPoints(int points)
    {
        if (text != null)
            text.text = $"+{points}";
    }
}
