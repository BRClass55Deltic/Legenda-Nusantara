using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;

    public float movementSpeed = 5.0f;
    public float sprintSpeed = 9.0f;

    public float jumpHeight = 3.0f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float gravity = -9.81f;

    Vector3 velocity;
    bool isGrounded;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // jump
        /*isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        /*if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }*/

        

        // Move
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Animation
        bool isWalking = move.magnitude > 0.01f;
        animator.SetBool("isWalking", isWalking);

        // Sprint
        float currentSpeed = movementSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && vertical > 0)
        {
            currentSpeed = sprintSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        //jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
