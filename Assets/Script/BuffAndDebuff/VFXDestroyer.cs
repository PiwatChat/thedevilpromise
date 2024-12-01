using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDestroyer : MonoBehaviour
{
    public float destroyDelay = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
}
