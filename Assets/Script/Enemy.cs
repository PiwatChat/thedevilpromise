using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;
    public GameObject popUpDamagePrefab;
    public TMP_Text damageText;
    public LayerMask playerLayer;
    public bool hitPlayer = true;
    public bool isHit = true;
    public float damage = 10f;
    public int exp = 25;
    public Animator animator;
    public string qusetname;
    
    private EnemyAI enemy;
    private Rigidbody2D rb;
    private Character character;
    private QuestManager questManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<EnemyAI>();
        character = FindObjectOfType<Character>();
        questManager = FindObjectOfType<QuestManager>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        health -= damage;
        damageText.text = "-"+damage.ToString();
        GameObject damagePopUp = Instantiate(popUpDamagePrefab, transform.position, Quaternion.identity);
        
        damagePopUp.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, -0.5f);
        if (health <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("isHurt");
            enemy.Knockback(knockbackDirection);
            StartCoroutine(HandleHit());
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDie");
        StartCoroutine(DieTo());
    }

    private IEnumerator DieTo()
    {
        enemy.enabled = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);
        character.AddExperience(exp);
        questManager.UpdateQuest(qusetname, 1);
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
            animator.SetTrigger("isAttack");
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
        yield return new WaitForSeconds(1.5f);
        isHit = true;
    }
}
