using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletEnemyboss : MonoBehaviour
{
    public float speed;
    private GameObject player;
    private Vector3 LastPos;
    private PlayerManager playerManager;
    private GameObject enemyBoss;
    private EnemyBoss boss;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyBoss = GameObject.FindGameObjectWithTag("Boss");
        boss = enemyBoss.GetComponent<EnemyBoss>();
        LastPos = (player.transform.position - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(LastPos * speed *Time.deltaTime);
        StartCoroutine(DestroyCD());
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") 
        {
            playerManager = other.gameObject.GetComponent<PlayerManager>();
            Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;
            playerManager.TakeDamage(boss.RangeDamge, knockbackDirection);
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyCD()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }
}
