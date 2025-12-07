using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePlayerController : MonoBehaviour
{
    public Joystick joystick;
    public JumpButton jumpButton;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    // ==== CROUCH ====
    public CrouchButton crouchButton;
    public bool isCrouching = false;

    public float standHeight = 1.8f;
    public float crouchHeight = 1.0f;

    public float crouchSpeed = 2.5f;
    private float originalSpeed;

    private CharacterController controller;
    private Transform cam;
    private Vector3 direction;

    private Vector3 velocity;
    private bool isGrounded;

    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();

        originalSpeed = moveSpeed;
    }

    void Update()
    {
        // ==== CEK GROUND ====
        isGrounded = Physics.CheckSphere(
            transform.position + Vector3.down * 0.1f,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // ==== HANDLE CROUCH ====
        if (crouchButton.pressed)
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchHeight;
                moveSpeed = crouchSpeed;

                animator.SetBool("isCrouching", true);
                animator.SetTrigger("crouchDown");
            }
            else
            {
                controller.height = standHeight;
                moveSpeed = originalSpeed;

                animator.SetBool("isCrouching", false);
                animator.SetTrigger("standUp");
            }

            crouchButton.pressed = false;
        }

        // ==== INPUT JOYSTICK ====
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 inputDir = new Vector3(horizontal, 0, vertical);
        bool isMoving = inputDir.sqrMagnitude > 0.01f;

        // ==== WALK STATE (NORMAL) ====
        // ONLY WALK IF NOT CROUCHING
        bool isWalking = !isCrouching && isMoving;
        animator.SetBool("isWalking", isWalking);

        // ==== CROUCH WALK ====
        bool isCrouchWalking = isCrouching && isMoving;
        animator.SetBool("isCrouchWalking", isCrouchWalking);

        // ==== MOVEMENT ====
        if (isMoving)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * vertical + camRight * horizontal;
            direction = moveDir.normalized;

            controller.Move(direction * moveSpeed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // ==== JUMP ====
        if (!isCrouching && jumpButton.pressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("isJumping", true);
        }

        if (isGrounded)
            animator.SetBool("isJumping", false);

        // ==== GRAVITY ====
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        jumpButton.pressed = false;
    }
}
