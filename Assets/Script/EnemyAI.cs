  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public float moveSpeed;
    public float chaseSpeed;
    public float chaseRange = 3.2f;
    public float attackRange = 1.5f;
    public float knockbackForce = 5f;
    public  bool isFacingRight = true;
    public Animator animator;
    

    private Rigidbody2D rb;
    private Transform currentPoint;
    private Transform player;
    private Enemy enemy;

    private bool isChasing = false;
    private bool isKnockback = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
        enemy = GetComponent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isKnockback)
        {
            if (isChasing)
            {
                ChasePlayer();
            }
            else
            {
                animator.SetBool("isPlayer", false);
                MoveBetweenPoints();
            }
        }
        
        
        if (Vector2.Distance(transform.position, player.position) < chaseRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
    }

    private void MoveBetweenPoints()
    {
        Vector2 point = currentPoint.position - transform.position;
        rb.velocity = new Vector2(point.x > 0 ? moveSpeed : -moveSpeed, 0);

        if (point.x > 0 && !isFacingRight || point.x < 0 && isFacingRight)
        {
            flip();
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            currentPoint = currentPoint == pointB.transform ? pointA.transform : pointB.transform;
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        
        
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            if (!isKnockback)
            {
                animator.SetBool("isPlayer", true);
                rb.velocity = Vector2.zero;
                AttackPlayer();
            }
        }
        else
        {
            rb.velocity = direction * chaseSpeed;
            
            if (direction.x > 0 && !isFacingRight || direction.x < 0 && isFacingRight)
            {
                animator.SetBool("isPlayer", false);
                flip();
            }
        }
        
        if (Vector2.Distance(transform.position, player.position) >= chaseRange)
        {
            isChasing = false;
            currentPoint = pointA.transform;
        }
    }
    
    private void AttackPlayer()
    {
        if (enemy.hitPlayer && enemy.isHit)
        {
            enemy.EnemyAttack();
            Debug.Log("Attacking Player!");
        }
    }
    
    public void Knockback(Vector2 direction)
    {
        rb.AddForce(-direction.normalized * knockbackForce, ForceMode2D.Impulse);
        isKnockback = true;
        
        StartCoroutine(ResetKnockback());
    }
    
    private IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.5f);
        isKnockback = false;
    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}
