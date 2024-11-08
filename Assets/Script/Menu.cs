using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public void Mmenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void Play()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
