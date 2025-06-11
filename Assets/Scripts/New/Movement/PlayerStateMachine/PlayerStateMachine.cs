using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerControls _playerControls;
    public Vector2 moveInput;
    public Vector3 velocity;
    public bool isJumpPressed;
    public bool isGrounded;
    private float turnVelocity;

    [Header("Character Controller")]
    public CharacterController characterController;

    [Header("Camera")]
    public Camera cam;

    [Header("Player Animator")]
    public Animator animator;

    [Header("Parameters")]
    public float Speed = 5f;
    public float TurnVelocity = 0.1f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.1f;
    public Transform groundCheck;
    public LayerMask gLayer;

    private PlayerBaseState currentState;
    private PlayerStateFactory stateFactory;

    void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.Movement.Keyboard.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        _playerControls.Movement.Keyboard.canceled += ctx => moveInput = Vector2.zero;
        _playerControls.Movement.Jump.performed += ctx => isJumpPressed = true;

        cam = Camera.main;

        stateFactory = new PlayerStateFactory(this);
        currentState = stateFactory.Idle();
        currentState.EnterState();
        
    }

    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    void Update()
    {
        UpdateGroundStatus();
        ApplyGravity();

        currentState.UpdateState();

        if (isJumpPressed)
        {
            currentState.HandleJumpInput();
            isJumpPressed = false;
        }
    }

    public void SwitchState(PlayerBaseState newState)
    {
        currentState.ExitState();
        currentState = newState;
        newState.EnterState();
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        return Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
    }

    public void RotateTowardsMovementDirection(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, TurnVelocity);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    void UpdateGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, gLayer);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void ApplyJumpVelocity()
    {
        velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
    }
}
