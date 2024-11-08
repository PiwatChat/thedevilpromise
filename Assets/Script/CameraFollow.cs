using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.5f;
    public float smoothTime = 0.25f;
    
    private float shakeTimer = 0f;
    
    public Vector3 offset = new Vector3(0f, 3f, -10f);
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;
    
    private void Update()
    {
        Vector3 targetPosition = target.position + offset;

        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            
            transform.localPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
        }
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}
