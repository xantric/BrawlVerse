using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = .2f;

    [SerializeField] private float groundDistance = .2f;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    [SerializeField] private float CameraXoffset;
    [SerializeField] private float CameraYoffset;
    [SerializeField] private float CameraZoffset;
    [SerializeField] private float senstivity;

    private Animator animator; // ✅ Animator reference

    bool isGrounded;
    Vector3 velocity;

    void Start()
    {
        // ✅ Get the Animator on this GameObject
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        
        // Check if player is grounded
        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, GroundLayer);

        // Reset downward velocity if grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * x + transform.forward * z;

        // Rotate player smoothly to face movement direction (if moving)
        if (move.magnitude > 0.1f)
        {
            Vector3 direction = new Vector3(move.x, 0f, move.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }

        // Move the player
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical velocity
        characterController.Move(velocity * Time.deltaTime);

        // Update animator Speed parameter based on input magnitude (0 idle, >0 running)
        float inputMagnitude = new Vector2(x, z).magnitude;
        animator.SetFloat("Speed", inputMagnitude);
    }

    
}
