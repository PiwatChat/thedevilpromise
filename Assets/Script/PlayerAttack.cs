using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss") && other.CompareTag("Enemy"))
        {
            StartCoroutine(DealDamage(other.GetComponent<Enemy>()));
        }
    }
    
    private IEnumerator DealDamage(Enemy enemy)
    {
        if (enemy.health <= 0)
        {
            yield break;
        }
            
        Vector2 knockbackDirection = (transform.position - enemy.transform.position).normalized;
        enemy.TakeDamage(10, knockbackDirection);
    }
}
