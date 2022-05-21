using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartTestADHD()
    {
        MainMenuParam.param = 1;
        SceneManager.LoadScene(6);
        //SceneManager.LoadScene(2);
    }
    
    public void StartTestDyslexia()
    {
        MainMenuParam.param = 2;
        SceneManager.LoadScene(6);
        //SceneManager.LoadScene(3);
    }
    
    public void StartTestSelfSufficiency()
    {
        MainMenuParam.param = 3;
        SceneManager.LoadScene(6);
        //SceneManager.LoadScene(4);
    }
    
    public void StartTestObesity()
    {
        MainMenuParam.param = 4;
        SceneManager.LoadScene(6);
        //SceneManager.LoadScene(5);
    }
    
	public void StartTestCompassion()
	{
        MainMenuParam.param = 5;
        SceneManager.LoadScene(6);
		//SceneManager.LoadScene(4);
	}
    
    public void StartTestEmbodiment()
    {
        MainMenuParam.param = 0;
        SceneManager.LoadScene(6);
    }

    public void StartCharacterCreatorUMA()
    {
        SceneManager.LoadScene("CharacterCreatorUMA");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
