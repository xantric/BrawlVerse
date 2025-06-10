using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform lookAt;
    [SerializeField] private Camera followCam;

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float sprintSpeed = 16f;
    [SerializeField] private float gravity_ascent = -9.81f;
    [SerializeField] private float gravity_descent = -19.62f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float groundDistance = .2f;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;
    bool isGrounded;
    Vector3 velocity;
    private float speed;
    public bool isRolling;
    private void Start() {
        speed = moveSpeed;
    }
    void Update() {
        Movement();
        Jump();
        Sprint();
        //Debug.Log(speed);
    }

    private void Movement() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * speed * Time.deltaTime);
        anim.SetFloat("speed", (move * speed * Time.deltaTime).magnitude);
        if (move != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
    private void Jump() {
        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, GroundLayer);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            velocity.y = Mathf.Sqrt(-2f * gravity_ascent * jumpHeight);

        }
        if (isGrounded && velocity.y < 0f) {
            velocity.y = -2f;
        }
        velocity.y += gravity_descent * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Sprint() {

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        speed = isSprinting? sprintSpeed : moveSpeed;
    }
}
