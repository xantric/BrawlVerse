using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalyerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    [SerializeField] private float groundDistance = .2f;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    [SerializeField] private float CameraXoffset;
    [SerializeField] private float CameraYoffset;
    [SerializeField] private float CameraZoffset;
    [SerializeField] private float senstivity;

    [SerializeField] private Animator _animator;

    [SerializeField] public float dodgeSpeed = 20f;
    [SerializeField] public float dodgeDuration = 0.8f;
    [SerializeField] public float rollDuration = 2.4f;
    private bool isDodging = false;
    public bool isRolling = false;

    public Transform camTransform;

    bool isGrounded;
    Vector3 velocity;

    private Transform _rootBone;
    private Vector3 _initialRootPosition;
    // Start is called before the first frame update
    void Start()
    {
        _rootBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _initialRootPosition = _rootBone.position;
        //camTransform = Camera.main.transform;
    }
    // Update is called once per frame
    void Update()
    {
        //Vector3 displacement = _rootBone.position - _initialRootPosition;
        //Debug.Log("Displacement: " + displacement);
        if (!isDodging)
        {

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = camForward * z + camRight * x;

            characterController.Move(move * moveSpeed * Time.deltaTime);

            if (move != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
            _animator.SetFloat("Speed", (move * moveSpeed * Time.deltaTime).magnitude);

            isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, GroundLayer);

            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);

            }
            if (isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
            {
                Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

                StartCoroutine(DelayedDodge());
            }

            else if (Input.GetKeyDown(KeyCode.RightShift))
            {

                StartCoroutine(Roll());
            }
        }
        velocity.y += gravity * Time.deltaTime;
        Debug.Log(velocity);
        characterController.Move(velocity * Time.deltaTime);


    }

    private IEnumerator Dodge()
    {
        isDodging = true;

        // _animator.SetTrigger("Dodge");
        Vector3 dodgeDirection = transform.forward;

        float elapsed = 0f;
        while (elapsed < dodgeDuration)
        {
            characterController.Move(dodgeDirection * dodgeSpeed* Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }
    private IEnumerator Roll()
    {
        isRolling = true;
        // _animator.applyRootMotion = false;
        _animator.SetBool("Roll",isRolling);
        Vector3 rollDirection = transform.forward;

        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            characterController.Move(rollDirection * dodgeSpeed / 1.5f * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isRolling = false;
        _animator.SetBool("Roll",isRolling);
        // _animator.applyRootMotion = true;
    }
    IEnumerator DelayedDodge()
    {
        yield return new WaitForSeconds(0.01f); // Delay of 0.1 seconds before dodging

        _animator.SetTrigger("Dodge");
        StartCoroutine(Dodge());
    }

}

