using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerControls _playerControls;
    
    private float turnVelocity;

    [Header("Character Controller")]
    public CharacterController characterController;

    [Header("Camera")]
    public Camera cam;

    [Header("Player Animator")]
    public Animator animator;
    public AnimatorOverrideController baseOverrideController;
    [HideInInspector] public AnimatorOverrideController runtimeOverride;

    [Header("Parameters")]
    public float Speed = 5f;
    public float TurnVelocity = 0.1f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.1f;
    public Transform groundCheck;
    public LayerMask gLayer;

    [Header("Enemy Layer")]
    public LayerMask EnemyLayer;

    [Header("Attack Data")]
    public List<AttackData> attacks;

    [Header("Attack Origins")]
    public List<AttackOriginEntry> attackOrigins = new();
    public Dictionary<string, Transform> attackOriginMap = new();

    private Dictionary<string, AttackData> attackMap;
    private Dictionary<string, float> cooldowns;

    [Header("Inputs")]
    // public variables for classes
    public Vector2 moveInput;
    public Vector3 velocity;
    public bool isJumpPressed;
    public bool isGrounded;
    public bool isAttacking;


    private PlayerBaseState currentState;
    private PlayerStateFactory stateFactory;

    void Awake()
    {
        InitializeControls();
        
        
        cam = Camera.main;

        runtimeOverride = new AnimatorOverrideController(baseOverrideController);
        animator.runtimeAnimatorController = runtimeOverride;

        // Set up attack maps
        SetUpAttackMaps();

        stateFactory = new PlayerStateFactory(this);
        currentState = stateFactory.Idle();
        currentState.EnterState();
        
    }
    void SetUpAttackMaps()
    {
        attackMap = new Dictionary<string, AttackData>();
        cooldowns = new Dictionary<string, float>();

        foreach (var atk in attacks)
        {
            attackMap[atk.inputActionName] = atk;
            cooldowns[atk.inputActionName] = 0f;
            var inputAction = _playerControls.Attack.Get().FindAction(atk.inputActionName);
            //Debug.Log(_playerControls.Attack.Get().actions.ToString());
            if (inputAction != null)
                inputAction.performed += ctx => OnAttackInput(atk);
            else
                Debug.LogWarning($"InputAction '{atk.inputActionName}' not found.");
        }
        // Set up Atk Origins as well
        foreach (var entry in attackOrigins)
        {
            if (!attackOriginMap.ContainsKey(entry.originName))
                attackOriginMap[entry.originName] = entry.originTransform;
        }
    }
    private void OnAttackInput(AttackData atk)
    {
        if (Time.time >= cooldowns[atk.inputActionName])
        {
            cooldowns[atk.inputActionName] = Time.time + atk.cooldown;
            SwitchState(stateFactory.Attack(atk));
        }
    }

    // Initialize controls
    void InitializeControls()
    {
        _playerControls = new PlayerControls();
        _playerControls.Movement.Keyboard.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        _playerControls.Movement.Keyboard.canceled += ctx => moveInput = Vector2.zero;
        _playerControls.Movement.Jump.performed += ctx => isJumpPressed = true;
        _playerControls.Attack.Headbutt.performed += OnHeadbuttPressed;
        
    }

    void OnHeadbuttPressed(InputAction.CallbackContext ctx)
    {
        isAttacking = ctx.ReadValueAsButton();
        if (isAttacking)
        {
            //SwitchState(stateFactory.Headbutt());
        }

    }

    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    
    void Update()
    {
        UpdateGroundStatus();
        ApplyGravity();
        HandleRotation();
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
    
    void HandleRotation()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, TurnVelocity);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public Vector3 GetMoveDirection(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        return moveDir.normalized;
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

    public void SetAttackActive(bool active) { isAttacking = active; }

    // Animation Events Refrences
    public void ApplyJumpVelocity()
    {
        velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
    }

    public void ApplyAttackDamage()
    {
        if (currentState is PlayerAttackState atk)
            atk.ApplyDamage();
    }

    public void EndAttack()
    {
        if (currentState is PlayerAttackState atk)
            SwitchState(stateFactory.Idle());
    }




}
