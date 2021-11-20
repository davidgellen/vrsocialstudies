using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartTestADHD()
    {
        SceneManager.LoadScene(2);
    }

    public void StartTestDyslexia()
    {
        SceneManager.LoadScene(3);
    }

    public void StartTestSelfSufficiency()
    {
        SceneManager.LoadScene(4);
    }

    public void StartTestObesity()
    {
        SceneManager.LoadScene(5);
    }

    public void StartTestEmbodiment()
    {
        SceneManager.LoadScene(6);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
