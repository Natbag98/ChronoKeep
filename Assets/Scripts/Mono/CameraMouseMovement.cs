using UnityEngine;

public class CameraMouseMove : MonoBehaviour
{
    public float sensitivity = 0.5f; // How sensitive the camera is to mouse movement
    public float maxAngle = 10f; // Maximum rotation angle

    private Vector2 mousePos;
    private Vector3 initialRotation;

    void Start()
    {
        // Store the camera's initial rotation
        initialRotation = transform.eulerAngles;
    }

    void Update()
    {
        // Get mouse position (normalized to -1 to 1 range)
        mousePos.x = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        mousePos.y = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Calculate rotation based on mouse position
        float rotX = -mousePos.y * maxAngle; // Tilt up/down
        float rotY = mousePos.x * maxAngle;  // Tilt left/right

        // Apply rotation with sensitivity
        transform.eulerAngles = initialRotation + new Vector3(rotX * sensitivity, rotY * sensitivity, 0);
    }
}