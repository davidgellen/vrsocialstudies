using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartTest()
    {
        //Prerobit, ze si moze uzivatel vybrat, ktory test absolvuje.
        // - cez setActive nastavit dalsie menu, ktore sa otvori a hlavne
        //schova. Po vybrati, sa otvori dany level.
        SceneManager.LoadScene(1);
    }

    public void Credits()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
