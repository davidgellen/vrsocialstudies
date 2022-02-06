using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MainMenu 
{
	public static int info = -1;
	
    public static void StartTestADHD()
    {
        SceneManager.LoadScene(2);
    }

    public static void StartTestDyslexia()
    {
        SceneManager.LoadScene(3);
    }

    public static void StartTestSelfSufficiency()
    {
		info = 5;
        SceneManager.LoadScene(4);
    }

    public static void StartTestObesity()
    {
        SceneManager.LoadScene(5);
    }

    public static void StartTestEmbodiment()
    {
        SceneManager.LoadScene(6);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
