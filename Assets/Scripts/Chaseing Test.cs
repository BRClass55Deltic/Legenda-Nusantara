using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseingTest : MonoBehaviour
{
    public GameObject Enemy;
    public GameObject Target;
    public float speed = 2f;

    private Animator animator;

    void Start()
    {
       animator = GetComponent<Animator>();
    }

    void Update()
    {
        Enemy.transform.position = Vector3.MoveTowards(Enemy.transform.position, Target.transform.position, speed * Time.deltaTime);
        animator.SetBool("isChasing", true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("CatchBool", true);
            Debug.Log("Player is Kill");
        }
        else
        {
            animator.SetBool("CatchBool", false);
        }
    }
}
