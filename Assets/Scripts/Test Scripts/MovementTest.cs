using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public CharacterController controller;
    public float movementSpeed = 5.0f;
    public float gravity = -0.2f;
    Vector3 velocity;

    // Jumping
   /* public float jumpHeight = 3.0f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;
    public GameObject escape; */

    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogWarning("AudioManager not found in the scene!");
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
       float horizontal = Input.GetAxis("Horizontal");
       float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * movementSpeed * Time.deltaTime);

        bool isMoving = move.sqrMagnitude > 0.01f;
    // === WALK SOUND (LOOP) ===
        if (isMoving)
            {
                audioManager.PlayLoopSFX(audioManager.WalkSFX);
            }
        else
            {
                audioManager.StopLoopSFX();
            }
        }
}
