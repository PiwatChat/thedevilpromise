using System.Collections;
using UnityEngine;

public class AllyAI : MonoBehaviour
{
    public float attackRange = 1.5f;          // ระยะโจมตีของ AI เพื่อน
    public float detectRange = 5f;            // ระยะการตรวจจับศัตรูของ AI เพื่อน
    public float damage = 15f;                // ค่าดาเมจที่ AI เพื่อนสามารถทำได้
    public float moveSpeed = 3f;              // ความเร็วในการเคลื่อนที่ของ AI เพื่อน
    public LayerMask enemyLayer;              // เลเยอร์ของศัตรูที่ AI จะโจมตี
    public Animator animator;

    private Rigidbody2D rb;
    private bool canAttack = true;
    private Transform targetEnemy;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ตรวจสอบว่า animator ถูกตั้งค่าแล้ว
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        DetectEnemy();
        MoveTowardsEnemy();
    }

    private void DetectEnemy()
    {
        // ค้นหาศัตรูภายในระยะ detectRange
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            // เลือกศัตรูที่ใกล้ที่สุด
            targetEnemy = enemiesInRange[0].transform;

            // ตรวจสอบว่าเป็นบอสหรือไม่
            if (targetEnemy.CompareTag("Boss"))
            {
                // ถ้าเป็นบอสให้ทำอะไรบางอย่าง (โจมตีหรือวิ่งเข้าไป)
                Attack();
            }
        }
        else
        {
            targetEnemy = null;
        }
    }

    private void MoveTowardsEnemy()
    {
        if (targetEnemy == null) return;

        // ตรวจสอบระยะทางระหว่าง AI กับศัตรู
        float distanceToEnemy = Vector2.Distance(transform.position, targetEnemy.position);

        if (distanceToEnemy > attackRange)
        {
            // เคลื่อนที่เข้าไปหาเป้าหมาย
            Vector2 direction = (targetEnemy.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            // ตั้งทิศทางการเคลื่อนไหวใน Animator
            animator.SetBool("isMoving", true);
        }
        else
        {
            // หยุดเคลื่อนไหวและโจมตีศัตรู
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);

            if (canAttack)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        animator.SetTrigger("isAttack");

        // ตรวจสอบว่าศัตรูมีคอมโพเนนต์ Enemy และทำดาเมจให้ศัตรู
        if (targetEnemy.CompareTag("Boss"))
        {
            Enemy bossEnemyComponent = targetEnemy.GetComponent<Enemy>();
            if (bossEnemyComponent != null)
            {
                Vector2 knockbackDirection = (targetEnemy.position - transform.position).normalized;
                bossEnemyComponent.TakeDamage(damage, knockbackDirection);
            }
        }
        else
        {
            // หากไม่ใช่บอส โจมตีศัตรูทั่วไป
            Enemy enemyComponent = targetEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                Vector2 knockbackDirection = (targetEnemy.position - transform.position).normalized;
                enemyComponent.TakeDamage(damage, knockbackDirection);
            }
        }

        // เริ่มการหน่วงเวลาก่อนที่จะโจมตีอีกครั้ง
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1.5f);  // ระยะเวลารอระหว่างการโจมตี
        canAttack = true;
    }

    // แสดงระยะตรวจจับใน Unity Editor เพื่อช่วยในการปรับแต่งระยะ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
