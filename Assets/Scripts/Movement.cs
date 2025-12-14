using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float rotationSpeed = 10f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Crouch")]
    public float standHeight = 1.8f;
    public float crouchHeight = 1.0f;
    public bool isCrouching = false;

    private float originalSpeed;
    private float currentSpeed;
    private bool sprinting;

    private CharacterController controller;
    private Transform cam;
    private Animator animator;
    private AudioManager audioManager;

    private Vector3 velocity;
    private Vector3 direction;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
        audioManager = FindObjectOfType<AudioManager>();

        originalSpeed = moveSpeed;
        currentSpeed = moveSpeed;

        if (audioManager == null)
            Debug.LogWarning("AudioManager tidak ditemukan!");
    }

    void Update()
    {
        // ===== GROUND CHECK =====
        isGrounded = Physics.CheckSphere(
            transform.position + Vector3.down * 0.1f,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // ===== INPUT =====
        float horizontal = Input.GetAxisRaw("Horizontal"); // A / D
        float vertical = Input.GetAxisRaw("Vertical");     // W / S

        Vector3 inputDir = new Vector3(horizontal, 0, vertical);
        bool isMoving = inputDir.sqrMagnitude > 0.01f;

        // ===== CROUCH =====
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrouch();
        }

        // ===== SPRINT =====
        if (Input.GetKey(KeyCode.LeftShift) && vertical > 0 && !isCrouching)
        {
            sprinting = true;
            currentSpeed = sprintSpeed;
        }
        else
        {
            sprinting = false;
            currentSpeed = moveSpeed;
        }

        // ===== ANIMATOR =====
        animator.SetBool("isWalking", isMoving && !isCrouching && !sprinting);
        animator.SetBool("isRunning", isMoving && sprinting);
        animator.SetBool("isCrouchWalking", isMoving && isCrouching);
        animator.SetBool("isCrouching", isCrouching);

        // ===== MOVEMENT =====
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

            controller.Move(direction * currentSpeed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // ===== GRAVITY (WAJIB ADA) =====
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // =========================
    // ===== CROUCH LOGIC ======
    // =========================
    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            controller.height = crouchHeight;
            moveSpeed = crouchSpeed;
            sprinting = false;
        }
        else
        {
            controller.height = standHeight;
            moveSpeed = originalSpeed;
        }
    }

    // =================================================
    // ===== FOOTSTEP (DIPANGGIL DARI ANIMATION EVENT) ==
    // =================================================
    public void PlayFootstep()
    {
        Debug.Log("FOOTSTEP DIPANGGIL");

        
        if (audioManager == null || !isGrounded)
            return;

        if (sprinting)
        {
            audioManager.PlaySFX(audioManager.runSFX);
        }
        else
        {
            audioManager.PlaySFX(audioManager.WalkSFX);
        }
    }
}
