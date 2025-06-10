using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MouseLookAround : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float mouseSensitivity = 100f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private Transform cameraTarget;

    void Start()
    {
        // The target the camera is following/looking at
        cameraTarget = virtualCamera.LookAt ? virtualCamera.LookAt : virtualCamera.Follow;

        if (cameraTarget == null)
        {
            Debug.LogError("Assign a Follow or LookAt target to the Virtual Camera.");
        }
        
        // Lock cursor for better camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationX += mouseX;
        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, -40f, 80f);  // Limit vertical look angle

        // Apply rotation to the camera target
        cameraTarget.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}
