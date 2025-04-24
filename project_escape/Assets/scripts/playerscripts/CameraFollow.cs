using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement
    public Vector3 offset; // Offset to maintain between the camera and the player

    void LateUpdate()
    {
        if (player != null)
        {
            // Target position based on player's position and offset
            Vector3 targetPosition = player.position + offset;

            // Smoothly interpolate between the current position and the target position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            // Update camera position
            transform.position = smoothedPosition;
        }
    }
}
