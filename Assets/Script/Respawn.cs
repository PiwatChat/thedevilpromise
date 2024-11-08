using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private PlayerManager playerManager;
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    public void RespawnPlayer()
    {
        playerManager.LoadPosition();
    }
}
