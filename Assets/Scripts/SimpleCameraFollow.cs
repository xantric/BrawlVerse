using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;       // Assign player transform here
    public Vector3 offset = new Vector3(0, 2, -4);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target.position + Vector3.up * 1.5f);  // Look slightly above player’s position
    }
}
