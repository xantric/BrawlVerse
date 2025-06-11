using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float sensitivity;
    public Transform motu;
    public float TopClamp = 60f;
    public float BottomClamp = -60f;
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        xRotation -= mouseY;
        
        xRotation = Mathf.Clamp(xRotation, TopClamp, BottomClamp);
        
        //Debug.Log(xRotation);
       
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        motu.Rotate(Vector3.up * mouseX);
    }
}
