using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermoment : MonoBehaviour
{
    // player moment
    public CharacterController cic;
    public float moveSpeed;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.01f;
    public Transform groundCheck;
    public LayerMask gLayer;
    public Animator onii;
    bool isGrounded;
    Vector3 velocity;




    private void Start()
    {
        cic = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // moment
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        cic.Move(move * moveSpeed * Time.deltaTime);
        onii.SetFloat("Speed", (move * moveSpeed * Time.deltaTime).magnitude);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, gLayer);
        if (isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }
        velocity.y += gravity * Time.deltaTime;
        cic.Move(velocity * Time.deltaTime);
    }

}
