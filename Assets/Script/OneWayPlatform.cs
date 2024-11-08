using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private GameObject currentoneWayPlatform;
    
    [SerializeField] private BoxCollider2D playerCollider;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentoneWayPlatform != null)
            {
                StartCoroutine(DisableCollider());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("OneWayPlatform"))
        {
            currentoneWayPlatform = other.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("OneWayPlatform"))
        {
            currentoneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollider()
    {
        BoxCollider2D platformCollider = currentoneWayPlatform.GetComponent<BoxCollider2D>();
        
        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
