using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private PlayerControls _playerControls;
    private Vector2 moveInput;
    private float turnVelocity;
    private Vector3 moveDir;
    private float currentSpeed;
    private bool isGrounded;
    private Vector3 velocity;
    private bool isJumping;

    [Header("Character Controller")]
    [SerializeField] private CharacterController characterController;

    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("PlayerAnimator")]
    [SerializeField] private Animator animator;

    [Header("Parameters")]
    [SerializeField] private float Speed;
    [SerializeField] private float TurnVelocity;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.01f;
    public Transform groundCheck;
    public LayerMask gLayer;

    void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.Movement.Keyboard.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        _playerControls.Movement.Keyboard.canceled += ctx => moveInput = Vector2.zero;

        _playerControls.Movement.Jump.performed += ctx => RunJumpAnim();
    }

    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, gLayer);

        // Gravity reset when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Apply gravity every frame
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Movement
        float horizontal = moveInput.x;
        float vertical = moveInput.y;
        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
        moveDir = Vector3.zero;

        if (dir.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, TurnVelocity);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * Speed * Time.deltaTime);
        }

        currentSpeed = (moveDir.normalized * Speed * Time.deltaTime).magnitude;
        if (isJumping)
        {
            animator.SetFloat("Speed", 0); // prevent run animation during jump
        }
        else
        {
            animator.SetFloat("Speed", currentSpeed);
        }
        if (isJumping && isGrounded)
        {
            isJumping = false;
        }
    }
    void RunJumpAnim()
    {
        if (isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
            //animator.SetTrigger("Jump");
            isJumping = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            bool grounded = Physics.CheckSphere(groundCheck.position, groundDistance, gLayer);
            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
