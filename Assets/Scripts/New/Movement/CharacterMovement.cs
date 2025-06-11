using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private PlayerControls _playerControls;
    private Vector2 moveInput;
    private float turnVelocity;
    private Vector3 velocity;
    private bool isJumping;

    private bool _isMovementPressed;
    private bool _isJumpPressed;

    [Header("Character Controller")]
    [SerializeField] private CharacterController characterController;

    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Player Animator")]
    [SerializeField] public Animator animator;

    [Header("Parameters")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float TurnVelocity = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask gLayer;

    private bool isGrounded;

    // Unity Callbacks
    void Awake()
    {
        InitializeControls();
    }

    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    void Update()
    {
        UpdateGroundStatus();
        ApplyGravity();
        HandleMovement();
        HandleJumpState();
        HandleRotation();
    }

    // --- Input Initialization ---
    void InitializeControls()
    {
        _playerControls = new PlayerControls();
        _playerControls.Movement.Keyboard.performed += OnMovementInput;
        _playerControls.Movement.Keyboard.canceled += ctx => moveInput = Vector2.zero;
        _playerControls.Movement.Jump.performed += OnJumpInput;
    }

    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        _isMovementPressed = moveInput != Vector2.zero;
    }

    void OnJumpInput(InputAction.CallbackContext ctx)
    {
        _isJumpPressed = ctx.ReadValueAsButton();
        if (_isJumpPressed)
            TryJump();
    }

    // --- Ground / Gravity ---
    void UpdateGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, gLayer);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep character grounded
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    // --- Movement ---
    void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDir = GetMoveDirection(direction);
            characterController.Move(moveDir * Speed * Time.deltaTime);

            if (!isJumping)
                animator.SetFloat("Speed", Speed);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    void HandleRotation()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, TurnVelocity);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    Vector3 GetMoveDirection(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        return moveDir.normalized;
    }

    // --- Jump ---
    void TryJump()
    {
        if (isGrounded)
        {
            isJumping = true;
            animator.SetTrigger("Jump");
        }
    }
    public void ApplyJumpVelocity()
    {
        velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
    }
    void HandleJumpState()
    {
        if (isJumping && isGrounded)
        {
            isJumping = false;
        }
    }

    // --- Gizmos ---
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
