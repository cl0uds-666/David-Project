using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;             // Walking speed
    public float jumpHeight = 1.0f;      // How high the player can jump
    public float gravity = -9.81f;       // Gravity value (negative)

    [Header("Ground Check")]
    public Transform groundCheck;        // Empty object at player's feet
    public float groundDistance = 0.4f;  // Radius of the sphere check
    public LayerMask groundMask;         // Which layers count as ground

    [Header("Mouse Look Settings")]
    public Transform playerCamera;       // Reference to the Camera child
    public float mouseSensitivity = 100f;
    // If your mouse is super sensitive, reduce this number

    private CharacterController controller;
    private float xRotation = 0f;        // Keep track of camera's vertical rotation
    private Vector3 velocity;            // For vertical velocity (gravity, jumps)
    private bool isGrounded;             // Check if player is on ground

    void Start()
    {
        // Get the CharacterController component on this GameObject
        controller = GetComponent<CharacterController>();

        // Optionally lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MouseLook();
        Movement();
    }

    void MouseLook()
    {
        // Get mouse input (old Input Manager)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (look up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f); // Prevent flipping camera 180°

        // Apply rotation to the camera
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (turn left/right)
        transform.Rotate(Vector3.up * mouseX);
    }

    void Movement()
    {
        // Check if we're on the ground (simple sphere check)
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            // Fallback if you didn't assign a groundCheck
            isGrounded = controller.isGrounded;
        }

        // Reset vertical velocity if grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get WASD input
        float x = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float z = Input.GetAxis("Vertical");   // W/S or Up/Down

        // Calculate direction relative to player orientation
        Vector3 move = transform.right * x + transform.forward * z;

        // Move the controller
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // v = sqrt(2 * jumpHeight * -gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move vertically
        controller.Move(velocity * Time.deltaTime);
    }
}
