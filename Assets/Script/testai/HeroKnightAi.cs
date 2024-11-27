using UnityEngine;

public class HeroKnightAI : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float followDistance = 5f;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] int damage = 10;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Transform targetEnemy;
    private bool isGrounded = false;

    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask wallLayer; // เพิ่มการใช้งาน Wall layer
    public LayerMask enemyLayer;
    private bool isAttacking = false;

    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_body2d.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    void Update()
    {
        CheckGroundedStatus();
        FindClosestEnemy();

        if (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.position) <= attackDistance)
        {
            // ไปโจมตีศัตรูหากอยู่ในระยะโจมตี
            AttackEnemy();
        }
        else if (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.position) > attackDistance)
        {
            // ติดตามศัตรูหากเจอศัตรูแต่ยังไม่อยู่ในระยะโจมตี
            MoveTowardsEnemy();
        }
        else
        {
            // ติดตามผู้เล่นหากไม่มีศัตรู
            FollowPlayer();
            CancelFreezeY();

        }
        if (isGrounded && targetEnemy == null)
        {
            m_animator.SetBool("Grounded", false); // เปลี่ยนเป็นแอนิเมชัน Idle
            m_animator.SetBool("Grounded", true); // ยกเลิกแอนิเมชันวิ่ง
        }
    }

    void CheckGroundedStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        targetEnemy = null;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, enemyLayer);
        foreach (Collider2D enemyCollider in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetEnemy = enemyCollider.transform;
            }
        }
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ถ้าระยะห่างเกิน 25, วาร์ปกลับมาหาผู้เล่น
        if (distanceToPlayer > 25f)
        {
            transform.position = player.position; // วาร์ปไปหาผู้เล่นทันที
        }
        // ถ้าระยะห่างระหว่าง 10 และ 25, เดินไปหาผู้เล่น
        else if (distanceToPlayer > 10f)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            GetComponent<SpriteRenderer>().flipX = direction.x < 0;

            // ตรวจสอบการชนกับพื้น (Ground layer)
            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, direction, 1f, groundLayer);

            if (groundHit.collider != null && isGrounded)
            {
                // ถ้ามีกำแพงและ Hero Knight อยู่บนพื้น, เดินไปยังผู้เล่น
                m_body2d.velocity = new Vector2(direction.x * m_speed, m_body2d.velocity.y);
                m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
            }

            // ตรวจสอบการชนกับกำแพง (Wall layer)
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);

            if (wallHit.collider != null && isGrounded)
            {
                // ถ้ามีกำแพง, Hero Knight กระโดดไปข้างหน้า
                if (m_body2d.velocity.y == 0) // ถ้าไม่อยู่ในอากาศ
                {
                    // กระโดดไปข้างหน้า
                    m_body2d.velocity = new Vector2(direction.x * m_speed * 2, m_jumpForce); // เพิ่มความเร็วใน X และ Y
                    m_animator.SetTrigger("Jump");
                }
            }
        }
        // ถ้าระยะห่างน้อยกว่า 10, ให้หยุดหรือทำกิจกรรมอื่นๆ เช่น โจมตี
        else
        {
            // คุณสามารถเพิ่มการหยุดเคลื่อนที่หรือให้ Hero Knight โจมตีที่นี่
            m_body2d.velocity = Vector2.zero; // หยุดเคลื่อนที่
        }
    }

    void MoveTowardsEnemy()
    {
        if (targetEnemy == null) return;

        Vector2 direction = (targetEnemy.position - transform.position).normalized;
        GetComponent<SpriteRenderer>().flipX = direction.x < 0;

        // ตรวจสอบการชนกับพื้น (Ground layer)
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, direction, 1f, groundLayer);

        if (groundHit.collider != null && isGrounded)
        {
            // ถ้ามีกำแพงและ Hero Knight อยู่บนพื้น, เดินไปยังศัตรู
            m_body2d.velocity = new Vector2(direction.x * m_speed, m_body2d.velocity.y);
            m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
        }

        // ตรวจสอบการชนกับกำแพง (Wall layer)
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);

        if (wallHit.collider != null && isGrounded)
        {
            // ถ้ามีกำแพง, Hero Knight กระโดดไปข้างหน้า
            if (m_body2d.velocity.y == 0) // ถ้าไม่อยู่ในอากาศ
            {
                // กระโดดไปข้างหน้า
                m_body2d.velocity = new Vector2(direction.x * m_speed * 2, m_jumpForce); // เพิ่มความเร็วใน X และ Y
                m_animator.SetTrigger("Jump");
            }
        }
    }


    void AttackEnemy()
    {
        if (targetEnemy == null) return;

        Vector2 direction = (targetEnemy.position - transform.position).normalized;
        GetComponent<SpriteRenderer>().flipX = direction.x < 0;

        if (Vector2.Distance(transform.position, targetEnemy.position) <= attackDistance)
        {
            m_animator.SetTrigger("Attack1");
            // Freeze Y axis when attacking
            m_body2d.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            // เพิ่มโค้ดการลดพลังชีวิตของศัตรูที่นี่
        }
    }
    void CancelFreezeY()
    {
        // ยกเลิกการล็อก Y เพื่อให้สามารถเคลื่อนที่ในแกน Y ได้
        m_body2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
