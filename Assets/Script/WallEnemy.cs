using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnemy : MonoBehaviour
{
    public GameObject nextAreaBlocker;
    public List<Enemy> enemies;
    public List<EnemyBoss> enemyBosses;
    
    void Start()
    {
        if (enemies.Count == 0 && enemyBosses.Count == 0)
        {
            Debug.LogWarning("No enemies in the area!");
        }
    }

    void Update()
    {
        CheckEnemies();
    }
    
    private void CheckEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null);
        enemyBosses.RemoveAll(enemyBoss => enemyBoss == null);

        if (enemies.Count == 0 && nextAreaBlocker != null && enemyBosses.Count == 0)
        {
            Debug.Log("All enemies defeated! Unblocking next area.");
            nextAreaBlocker.SetActive(false);
        }
    }
}
