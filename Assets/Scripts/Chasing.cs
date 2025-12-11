using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chasing : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public string playerTag = "Player";

    public Camera mainCamera;
    public Camera caughtCamera;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

     public GameObject boo;

    private bool isCaught = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Set kecepatan agent
        agent.speed = moveSpeed;

        // Kamera default
        if (mainCamera != null) mainCamera.enabled = true;
        if (caughtCamera != null) caughtCamera.enabled = false;

        // Cari target player berdasarkan tag
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null)
        {
            player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || isCaught) return;

        ChasePlayer();
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);

        animator.SetBool("isChasing", true);

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity);
        }
    }

    // ========= TRIGGER TERTANGKAP =========
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isCaught = true;

            // Stop NavMesh
            agent.isStopped = true;
            agent.ResetPath();

            // Animasi ketangkap (pakai bool yang kamu punya)
            animator.SetBool("CatchBool", true);

            Debug.Log("Player is Kill");

            // Disable movement player (jika ada script movement)
            MonoBehaviour movement = other.GetComponent<MonoBehaviour>();
            if (movement != null) movement.enabled = false;

            // Switch kamera
            if (mainCamera != null) mainCamera.enabled = false;
            if (caughtCamera != null) caughtCamera.enabled = true;

            boo.SetActive(true);
        }
    }
}