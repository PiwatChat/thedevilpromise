using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemybossProj : MonoBehaviour
{
    public Transform playerpos;
    public GameObject Bullet;
    public float Shoot;
    public float StartCD;
    // Start is called before the first frame update
    void Start()
    {
        Shoot = StartCD;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(playerpos.position.x-transform.position.x, playerpos.position.y-transform.position.y);
        transform.up = direction;
        if (Shoot <= 0) 
        {
            Instantiate(Bullet,transform.position,transform.rotation);
            Shoot=StartCD;
        }
        else 
        {
            Shoot-= Time.deltaTime;
        }
    }
}
