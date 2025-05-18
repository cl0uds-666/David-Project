using UnityEngine;

public class PerkSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f; // Speed of rotation in degrees per second

    void Update()
    {
        // Rotate the player around the Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
