using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public int upgradeCost;
    private Status status;
    private Character character;
    
    void Start()
    {
        status = FindObjectOfType<Status>();
        character = FindObjectOfType<Character>();
        
        gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void UpgradePlayer()
    {
        status.UpgradeStatus(upgradeCost);
        character.skillPoint--;
    }
}
