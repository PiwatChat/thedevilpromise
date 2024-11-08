using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    public Transform player;
    public float stopDistance = 1f;
    public float smoothTime = 0.25f;
    public LayerMask wallLayer;
    
    public Vector3 offset = new Vector3(0f, 3f, -10f);
    
    private CameraFollow cameraFollow;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        cameraFollow = GetComponent<CameraFollow>();
    }

    void Update()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(player.position, Vector2.right, stopDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(player.position, Vector2.left, stopDistance, wallLayer);
        
        Vector3 targetPos = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);
        
        if (hitRight.collider != null || hitLeft.collider != null)
        {
            cameraFollow.enabled = false;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        }
        else
        {
            cameraFollow.enabled = true;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(player.position, stopDistance);
    }
}
