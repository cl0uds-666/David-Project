using UnityEngine;

public class Dice : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(45f, 90f, 30f); // degrees per second

    void Update()
    {
        // Rotate the cube based on rotationSpeed and time
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
