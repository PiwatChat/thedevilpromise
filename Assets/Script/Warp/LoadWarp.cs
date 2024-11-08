using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class LoadWarp : MonoBehaviour
{
    public Image fadeImage;
    public GameObject warpPosition;
    public float fadeDuration = 1.5f;
    public float currentGroundSoundCount;
    
    private CinemachineVirtualCamera virtualCamera;
    private GameObject player;
    private PlayerManager playerManager;

    private void Start()
    {
        //virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerManager = player.GetComponent<PlayerManager>();
            playerManager.currentGroundSoundCount = currentGroundSoundCount;
            StartCoroutine(WarpPlayer());
        }
    }
    
    private IEnumerator WarpPlayer()
    {
        //virtualCamera.Follow = null;
        
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(0, 0, 0, 1);

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        player.transform.position = warpPosition.transform.position;

        playerManager.PlayBackgroundSound();
        
        yield return new WaitForSecondsRealtime(0.5f);
        
        Time.timeScale = 0;
        
        yield return new WaitForSecondsRealtime(2f);
        
        Time.timeScale = 1;
        
        //virtualCamera.Follow = player.transform;
        
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(targetColor, startColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }
}
