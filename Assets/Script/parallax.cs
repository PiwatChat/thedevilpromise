using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    private Material mat;
    float distancia;
    
    [Range(0f,0.5f)]
    public float speed = 0.2f;
    
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    
    void Update()
    {
        distancia += Time.deltaTime * speed;
        mat.SetTextureOffset("_MainTex", Vector2.right * distancia);
    }
}
