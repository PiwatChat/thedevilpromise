using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.SavePosition(transform.position);
                Debug.Log("Saved player position: " + transform.position);
            }
        }
    }
}
