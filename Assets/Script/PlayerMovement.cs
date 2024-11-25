using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float jumpingPower = 10f;
    public float speed = 8f;
    public float runningSpeedMultiplier = 1.5f;
    public float fallMultiplier = 5f;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    public float wallSlideSpeed = 2f;
    public float wallJumpingTime = 0.2f;
    public float attackRange = 0.5f;
    public float attackDamage = 10f;
    public float heavyAttackDamage = 25f;
    public float heavyAttackCooldown = 15f;
    public float cooldownAttack = 1.5f;
    public Animator animator;
    public bool canAttack = true;
    public bool isHeavyAttacking = false;
    public float knockbackForce = 1.5f;
    public bool isDashing;
    public GameObject heavyAttackBar;
    public Slider heavyAttackFillBar;
    public AudioClip attackSound1;
    public AudioClip attackSound2;
    public AudioClip chargeSound;
    public AudioClip jumpSound;
    public AudioClip takeDamageSound;
    public AudioSource audioSourceCombat;
    
    private float horizontal;
    private bool isFacingRight = true;
    private bool canDash = true;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);
    private bool canHAttack = true;
    private int attackCount = 0;
    private Status playerStatus;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayers;

    private void Start()
    {
        playerStatus = GetComponent<Status>();
        heavyAttackFillBar.value = 1;
    }

    void Update()
    {
        if (isHeavyAttacking)
        {
            if (isDashing)
            {
                return;
            }
        }
        if (isDashing)
        {
            return;
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("isRun", Mathf.Abs(horizontal));
        
        if (Input.GetKey(KeyCode.Space) && Mathf.Abs(horizontal) < 0.01 && !IsGrounded() && !IsWalled() && !isDashing)
        {
            animator.SetBool("isJump", true);
            animator.SetBool("isJumpF", false);
        }

        else if (Input.GetKey(KeyCode.Space) && Mathf.Abs(horizontal) > 0.01 && !IsGrounded() && !IsWalled() && !isDashing)
        {
            animator.SetBool("isJumpF", true);
            animator.SetBool("isJump", false);
        }
        else if (IsGrounded() || IsWalled() || isDashing)
        {
            animator.SetBool("isJumpF", false);
            animator.SetBool("isJump", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            PlayJumpSound();
        }

        if (Input.GetKeyUp(KeyCode.Space) && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            StartCoroutine(Dash());
        }
        
        if (Input.GetMouseButtonDown(0) && canAttack && !playerStatus.openUIStatus)
        {
            StartCoroutine(CooldownAttack());
        }

        if (Input.GetKey(KeyCode.Q) && canHAttack)
        {
            StartCoroutine(CooldownHeavyAttack());
        }
        
        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
        ApplyFallMultiplier();
    }

    private void FixedUpdate()
    {
        if (isHeavyAttacking)
        {
            if (isDashing)
            {
                return;
            }
        }
        
        if (isDashing)
        {
            return;
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            runningSpeedMultiplier = playerStatus.agility + 5;
            rb.velocity = new Vector2(horizontal * runningSpeedMultiplier, rb.velocity.y);
        }
        else if (!isWallJumping)
        {
            speed = playerStatus.agility;
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded())
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            
            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            animator.SetBool("isJumpF", true);
            animator.SetBool("isJump", false);
            PlayJumpSound();
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }
    private void PlayJumpSound()
    {
        if (jumpSound != null && audioSourceCombat != null)
        {
            audioSourceCombat.PlayOneShot(jumpSound);
        }
    }

    private void StopWallJumping()
    {
        animator.SetBool("isJumpF", false);
        isWallJumping = false;
    }

    private void ApplyFallMultiplier()
    {
        rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        PlayJumpSound();
        
        rb.velocity = new Vector2(transform.localScale.x* dashingPower, 0f);
        if (IsGrounded())
        {
            animator.SetBool("isdash", true);
            animator.SetBool("isJump", false);
        }
        else
        {
            animator.SetBool("isAirDash", true);
            animator.SetBool("isJump", false);
            animator.SetBool("isJumpF", false);
        }
        trail.emitting = true;
        
        yield return new WaitForSeconds(dashingTime);
        
        trail.emitting = false;
        animator.SetBool("isdash", false);
        animator.SetBool("isAirDash", false);
        rb.gravityScale = originalGravity;
        isDashing = false;
        
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    
    private void Attack()
    {
        //canAttack = false;
        PerformAttack();
        
        Vector2 attackDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, attackDirection, attackRange, enemyLayers);
    
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    StartCoroutine(DealDamageEnemy(enemy, "Attack"));
                }

                EnemyBoss enemyBoss = hit.collider.GetComponent<EnemyBoss>();
                if (enemyBoss != null)
                {
                    StartCoroutine(DealDamageBoss(enemyBoss, "Attack"));
                }
            }
        }
    }

    private IEnumerator OnHit(float cd)
    {
        yield return new WaitForSeconds(cd);
        HitStop.Stop(0.15f);
    }
    
    private void HeavyAttack()
    {
        isHeavyAttacking = true;
        //canHAttack = false;
        animator.SetTrigger("HAttack");
        StartCoroutine(SoundHAttack());

        Vector2 attackDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, attackDirection, attackRange, enemyLayers);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    StartCoroutine(DealDamageEnemy(enemy, "HAttack"));
                }

                EnemyBoss enemyBoss = hit.collider.GetComponent<EnemyBoss>();
                if (enemyBoss != null)
                {
                    StartCoroutine(DealDamageBoss(enemyBoss, "HAttack"));
                }
            }
        }
    }

    private IEnumerator SoundHAttack()
    {
        audioSourceCombat.PlayOneShot(chargeSound);
        yield return new WaitForSeconds(1.2f);
        audioSourceCombat.Stop();
        audioSourceCombat.PlayOneShot(attackSound2);
    }
    
    private void PerformAttack()
    {
        attackCount = (attackCount % 3) + 1;
        
        switch (attackCount)
        {
            case 1:
                animator.SetTrigger("Attack1");
                audioSourceCombat.PlayOneShot(attackSound1);
                break;
            case 2:
                animator.SetTrigger("Attack2");
                audioSourceCombat.PlayOneShot(attackSound1);
                break;
            case 3:
                animator.SetTrigger("Attack3");
                audioSourceCombat.PlayOneShot(attackSound2);
                break;
        }
    }

    private IEnumerator CooldownAttack()
    {
        canAttack = false;
        Attack();
        yield return new WaitForSeconds(cooldownAttack);
        canAttack = true;
    }
    
    private IEnumerator CooldownHeavyAttack()
    {
        /*canHAttack = false;
        HeavyAttack();
        yield return new WaitForSeconds(15f);
        //isHeavyAttacking = false;
        canHAttack = true;*/
        
        canHAttack = false;
        heavyAttackBar.SetActive(true);
        heavyAttackFillBar.value = 0;
        HeavyAttack();
    
        float elapsedTime = 0f;
        while (elapsedTime < heavyAttackCooldown)
        {
            elapsedTime += Time.deltaTime;
            heavyAttackFillBar.value = 0 + (elapsedTime / heavyAttackCooldown);
            yield return null;
        }

        canHAttack = true;
        heavyAttackFillBar.value = 1;
        heavyAttackBar.SetActive(false);
    }
    
    private IEnumerator DealDamageEnemy(Enemy enemy , string attack)
    {
        if (!canHAttack && attack == "HAttack")
        {
            yield return new WaitForSeconds(1.2f);
            if (enemy.health <= 0)
            {
                yield break;
            }
            Vector2 knockbackDirection = (transform.position - enemy.transform.position).normalized;
            HitStop.Stop(0f);
            enemy.TakeDamage(playerStatus.strength + 15, knockbackDirection);
            isHeavyAttacking = false;
            yield break;
        }
        
        if(!canAttack && attack == "Attack")
        {
            if (enemy.health <= 0)
            {
                yield break;
            }
            
            Vector2 knockbackDirection = (transform.position - enemy.transform.position).normalized;
            enemy.TakeDamage(playerStatus.strength, knockbackDirection);
            StartCoroutine(OnHit(0.2f));
        }
    }
    
    private IEnumerator DealDamageBoss(EnemyBoss enemy , string attack)
    {
        if (!canHAttack && attack == "HAttack")
        {
            yield return new WaitForSeconds(1.2f);
            if (enemy.health <= 0)
            {
                yield break;
            }
            Vector2 knockbackDirection = (transform.position - enemy.transform.position).normalized;
            HitStop.Stop(0f);
            enemy.TakeDamage(playerStatus.strength + 15, knockbackDirection);
            isHeavyAttacking = false;
            yield break;
        }
        
        if(!canAttack && attack == "Attack")
        {
            if (enemy.health <= 0)
            {
                yield break;
            }
            
            Vector2 knockbackDirection = (transform.position - enemy.transform.position).normalized;
            enemy.TakeDamage(playerStatus.strength, knockbackDirection);
            StartCoroutine(OnHit(0.2f));
        }
    }
    
    public void Knockback(Vector2 direction)
    {
        rb.AddForce(-direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Vector2 attackDirection = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)attackDirection * attackRange);
    }
}
