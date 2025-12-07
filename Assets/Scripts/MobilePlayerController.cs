using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void Update()
    {
        // ==== CEK GROUND ====
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.1f,
                                         groundDistance,
                                         groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // ==== INPUT JOYSTICK ====
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 inputDir = new Vector3(horizontal, 0, vertical);

        bool isMoving = inputDir.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            // ==== CAMERA RELATIVE MOVEMENT ====
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * vertical + camRight * horizontal;
            direction = moveDir.normalized;

            controller.Move(direction * moveSpeed * Time.deltaTime);

            // rotasi menghadap arah gerakan
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // ==== JUMP ====
        if (jumpButton.pressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("isJumping", true);
        }

        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
        }

        // ==== GRAVITY ====
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Reset button
        jumpButton.pressed = false;
    }
}