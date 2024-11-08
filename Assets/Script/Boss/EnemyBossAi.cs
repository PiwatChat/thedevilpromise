using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossAi : MonoBehaviour
{

    public GameObject pointA;
    public GameObject pointB;
    public float moveSpeed;
    public float chaseSpeed;
    public float chaseRange = 3.2f;
    public float attackRange = 1.5f;
    public float attackProjRange = 50f;
    public float knockbackForce = 5f;
    public GameObject Bullet;
    public bool isFacingRight = true;
    public Animator animator;
    public bool isNotHit = true;

    private Rigidbody2D rb;
    private Transform currentPoint;
    private Transform player;
    private EnemyBoss enemy;

    private bool isChasing = false;
    private bool isKnockback = false;
    private bool isAttackingBullet = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
        enemy = GetComponent<EnemyBoss>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isKnockback)
        {
            if (isChasing)
            {
                animator.SetBool("isWalk", true);
                ChasePlayer();
            }
            else
            {
                animator.SetBool("isWalk", false);
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
                rb.velocity = Vector2.zero;
                AttackPlayer();
            }
        }
        else if (Vector2.Distance(transform.position, player.position) <= attackProjRange && isAttackingBullet)
        {
            if (!isKnockback)
            {
                rb.velocity = Vector2.zero;
                AttackProj();
            }
        }
        else
        {
            rb.velocity = direction * chaseSpeed;

            if (direction.x > 0 && !isFacingRight || direction.x < 0 && isFacingRight)
            {
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
    private void  AttackProj()
    {
        if (enemy.hitPlayer && isNotHit)
        {
            StartCoroutine(CDRange());
            Debug.Log("Attacking range Player!");
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
        Gizmos.DrawWireSphere(transform.position, attackProjRange);
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
    public IEnumerator CDRange()
    {
        animator.SetTrigger("bullet");
        isNotHit = false;
        yield return new WaitForSeconds(1f);
        isAttackingBullet = false;
        Instantiate(Bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(10f);
        isAttackingBullet = true;
        isNotHit = true;
    }
}

