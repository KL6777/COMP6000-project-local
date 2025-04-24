using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement

    private Vector2 movement;  // Stores movement input

    void Update()
    {
        // Get input for horizontal and vertical movement
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right Arrow
        movement.y = Input.GetAxisRaw("Vertical");   // W/S or Up/Down Arrow

        // Normalize movement to ensure diagonal speed is consistent
        movement = movement.normalized;

        // Update animator parameters
    }

    void FixedUpdate()
    {
        // Move the player while keeping the z-axis constant
        if (movement != Vector2.zero)
        {
            Vector3 newPosition = transform.position + (Vector3)(movement * moveSpeed * Time.fixedDeltaTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, -1f);
        }
    }
}
