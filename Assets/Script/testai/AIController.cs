using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public Transform player;
    public float followDistance = 5f;
    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;           // แรงกระโดด
    public int damage = 10;

    private Transform targetEnemy;
    private Rigidbody2D rb;
    private bool isGrounded = false;       // ตรวจสอบว่า AI อยู่บนพื้นหรือไม่

    public Transform groundCheck;          // จุดตรวจสอบการสัมผัสพื้น (เช่น ส่วนล่างของ AI)
    public LayerMask groundLayer;          // เลเยอร์ของพื้น
    public LayerMask enemyLayer; // กำหนดเลเยอร์ของศัตรูใน Inspector


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGroundedStatus();
        FindClosestEnemy();

        if (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.position) <= attackDistance)
        {
           // AttackEnemy();
        }
        else
        {
            FollowPlayer();
        }
    }

    void CheckGroundedStatus()
    {
        // ตรวจสอบว่าตัวละครอยู่บนพื้นโดยใช้ Raycast จากตำแหน่ง groundCheck ลงไปหาเลเยอร์พื้น
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        targetEnemy = null;

        // ค้นหา Collider2D ทั้งหมดในเลเยอร์ของศัตรู
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, enemyLayer);

        foreach (Collider2D enemyCollider in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
            if (distance < closestDistance)
            {
                Debug.Log("Enenmy");
                closestDistance = distance;
                targetEnemy = enemyCollider.transform;
            }
        }
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > followDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // ตรวจสอบว่ามีสิ่งกีดขวางด้านหน้า และถ้า AI อยู่บนพื้นให้ทำการกระโดด
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, groundLayer);
            if (hit.collider != null && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else
            {
                transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }

   
    


}
