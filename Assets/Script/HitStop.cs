using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

public class HitStop : MonoBehaviour
{
    public static void Stop(float duration)
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        instance.StartCoroutine(HitStopCoroutine(duration));
    }

    private static IEnumerator HitStopCoroutine(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }
    
    private static HitStop _instance;
    private static HitStop instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("HitStopManager");
                _instance = go.AddComponent<HitStop>();
                Object.DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Destroy(gameObject);
        }
    }
}
