using UnityEngine;

public class PowerupFloat : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float floatHeight = 0.25f;
    public float rotationSpeed = 90f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Bob up and down
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + new Vector3(0, offset, 0);

        // Rotate
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
