    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Movement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;
        public float groundDistance = 0.3f;
        public LayerMask groundMask;

        // Crouch
        public bool isCrouching = false;
        public float standHeight = 1.8f;
        public float crouchHeight = 1.0f;
        public float crouchSpeed = 2.5f;
        private float originalSpeed;

        // Sprint
        public float sprintSpeed = 8f;
        private bool sprinting = false;
        private float currentSpeed;

        private CharacterController controller;
        private Transform cam;

        private Vector3 velocity;
        private Vector3 direction;

        private bool isGrounded;
        private Animator animator;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            cam = Camera.main.transform;
            animator = GetComponentInChildren<Animator>();

            originalSpeed = moveSpeed;
            currentSpeed = moveSpeed;
        }

        void Update()
        {
            isGrounded = Physics.CheckSphere(
                transform.position + Vector3.down * 0.1f,
                groundDistance,
                groundMask
            );

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            float horizontal = Input.GetAxisRaw("Horizontal");   // A / D
            float vertical = Input.GetAxisRaw("Vertical");       // W / S

            Vector3 inputDir = new Vector3(horizontal, 0, vertical);
            bool isMoving = inputDir.sqrMagnitude > 0.01f;

            bool isWalking = !isCrouching && isMoving;
            animator.SetBool("isWalking", isWalking);

            bool isCrouchWalking = isCrouching && isMoving;
            animator.SetBool("isCrouchWalking", isCrouchWalking);

            currentSpeed = moveSpeed;

            if (Input.GetKey(KeyCode.LeftShift) && vertical > 0 && !isCrouching)
            {
                sprinting = true;
                animator.SetBool("isRunning", true);
            }
            else
            {
                sprinting = false;
                animator.SetBool("isRunning", false);
            }

            if (sprinting)
                currentSpeed = sprintSpeed;

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

            if (!isCrouching && Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("isJumping", true);
            }

            if (isGrounded)
                animator.SetBool("isJumping", false);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ToggleCrouch();
            }
        }


        void ToggleCrouch()
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchHeight;
                moveSpeed = crouchSpeed;
                animator.SetBool("isCrouching", true);

                sprinting = false;
            }
            else
            {
                controller.height = standHeight;
                moveSpeed = originalSpeed;
                animator.SetBool("isCrouching", false);
            }
        }
    }