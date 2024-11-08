using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    public List<Item> itemsToDrop;
    private Enemy enemy;
    private bool drop = false;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (enemy.health <= 0 && !drop)
        {
            drop = true;
            DropLoot();
        }
    }
    
    public void DropLoot()
    {
        float totalChance = 0f;
        foreach (var item in itemsToDrop)
        {
            totalChance += item.dropChance;
        }

        float randomValue = Random.Range(0f, totalChance);

        foreach (var item in itemsToDrop)
        {
            if (randomValue < item.dropChance)
            {
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                Debug.Log($"Dropped item: {item.itemName}");
                break;
            }
            randomValue -= item.dropChance;
        }
    }
}
