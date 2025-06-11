using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform player; // Player Transform to follow
    public float distance = 5.0f;
    public float height = 2.0f;

    public float sensitivityX = 3.0f; // Horizontal mouse sensitivity
    public float sensitivityY = 3.0f; // Vertical mouse sensitivity

    public float yMinLimit = -35f;
    public float yMaxLimit = 60f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (player)
        {
            // Mouse input
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, yMinLimit, yMaxLimit);

            // Calculate rotation and position
            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
            Vector3 offset = rotation * new Vector3(0.0f, 0.0f, -distance);
            Vector3 targetPosition = player.position + Vector3.up * height + offset;

            transform.position = targetPosition;
            transform.rotation = rotation;
        }
    }
}
