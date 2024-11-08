using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public float health = 100;
    public GameObject popUpDamagePrefab;
    public TMP_Text damageText;
    public LayerMask playerLayer;
    public bool hitPlayer = true;
    public bool isHit = true;
    
    public float damage = 10f;
    public float RangeDamge = 30f;
    public Animator animator;

    private EnemyBossAi enemy;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<EnemyBossAi>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        health -= damage;
        damageText.text = "-" + damage.ToString();
        GameObject damagePopUp = Instantiate(popUpDamagePrefab, transform.position, Quaternion.identity);
        damagePopUp.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, -0.5f);
        if (health <= 0)
        {
            Die();
        }
        else
        {
            
            enemy.Knockback(knockbackDirection);
            StartCoroutine(HandleHit());
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        StartCoroutine(DieTo());
    }

    private IEnumerator DieTo()
    {
        enemy.enabled = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private IEnumerator DealDamage(PlayerManager player)
    {
        if (player.currentHealth <= 0)
        {
            yield break;
        }
        
        if (hitPlayer && isHit)
        {
            Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;
            animator.SetTrigger("Attack");
            player.TakeDamage(damage, knockbackDirection);
            hitPlayer = false;
        }

        yield return new WaitForSeconds(3f);
        hitPlayer = true;
    }


    public void EnemyAttack()
    {
        if (!isHit)
        {
            return;
        }

        Vector2 attackDirection = enemy.isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDirection, enemy.attackRange, playerLayer);
        if (hit.collider != null)
        {
            StartCoroutine(DealDamage(hit.collider.gameObject.GetComponent<PlayerManager>()));
        }
    }

    private IEnumerator HandleHit()
    {
        isHit = false;
        yield return new WaitForSeconds(7f);
        isHit = true;
    }
    
}
