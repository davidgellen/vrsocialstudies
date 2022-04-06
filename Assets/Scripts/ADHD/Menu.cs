using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject renderCharacter; //Show character
    public GameObject[] characters; //Adding all characters

    public void SelectCharacter(int selection)
    {
        if (selection < characters.Length)
        {
            for(int i = 0; i < characters.Length; i++)
            {
                if (i == selection)
                    characters[i].SetActive(true);
                else
                    characters[i].SetActive(false);
            }
            PlayerPrefs.SetInt("choosen_avatar", selection); //PlayerPrefs.GetInt("choosen_avatar");
        }
    }

    public void LoadADHD()
    {
        SceneManager.LoadScene(1);
    }

    void Start()
    {
        SelectCharacter(0);
    }

    void Update()
    {
        renderCharacter.transform.RotateAround(renderCharacter.transform.position, renderCharacter.transform.up, Time.deltaTime * 15f); //Rotation object for better vizualization
    }
}