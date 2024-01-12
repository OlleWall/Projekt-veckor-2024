using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("NEXT-SCENE-1");
    }

    public void OptionScreen()
    {
        SceneManager.LoadScene("Options");
    }

    public void CreditScreen()
    {
        SceneManager.LoadScene("Credits");
    }

    public void BackButtons()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
