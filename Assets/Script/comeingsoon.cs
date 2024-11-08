using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class comeingsoon : MonoBehaviour
{
    public GameObject UiComeingsoon;
    public float fadeDuration = 1.5f;
    public Image fadeImage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ComeingSoon());
        }
    }

    private IEnumerator ComeingSoon()
    {
        UiComeingsoon.SetActive(true);
        
        yield return new WaitForSeconds(3f);
        
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(0, 0, 0, 1);
        
        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(0);
    }
}
