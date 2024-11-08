using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    private float vertical;
    private float speed = 8;
    private bool isLadder;
    private bool isClimbing;
    private PlayerMovement player;
    
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    private void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        vertical = Input.GetAxis("Vertical");

        if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
            animator.SetBool("isJump", false);
            animator.SetBool("isJumpF", false);
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }

        else if (!player.isDashing)
        {
            rb.gravityScale = 1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}
