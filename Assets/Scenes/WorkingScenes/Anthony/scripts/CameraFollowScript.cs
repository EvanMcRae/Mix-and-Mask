using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Drag your Player into this slot
    public Vector3 offset = new Vector3(0, 10, -5); // Position relative to player
    public float smoothSpeed = 0.125f;

    public Vector3 lookOffset = new Vector3(0, 2f, 0);

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position based on player's current position + offset
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move the camera to that position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Optional: Keep the camera looking at the player
        transform.LookAt(target.position + lookOffset);
    }
}