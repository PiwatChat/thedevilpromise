using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamge : MonoBehaviour
{
    private PlayerManager playerManager;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            playerManager.TakeDamage(100f, knockbackDirection);
        }
    }
}
