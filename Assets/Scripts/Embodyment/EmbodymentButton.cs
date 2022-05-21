using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmbodymentButton : MonoBehaviour
{
    public float time = 0;
    public float timeLimit = 2 * 60.0f;
    public TextMesh timeText;
    
    // Start is called before the first frame update
    void Start()
    {
        if (MainMenuParam.param == 0) // 10 min cisto embodyment
        {
            timeLimit = 10 * 60.0f;
        }
        else
        {
            timeLimit = 2 * 60.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time = time + Time.deltaTime;
        
        // show time
        timeLimit = timeLimit - Time.deltaTime;
        Debug.Log(timeLimit);
        
        float minutes = Mathf.FloorToInt(timeLimit / 60);
        float seconds = Mathf.FloorToInt(timeLimit % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (timeLimit <= 1)
        {
            StartSceneFromParam();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "button")
        {
            Debug.Log("click");
            //SceneManager.LoadScene("SingleRoom2");
            StartSceneFromParam();
        }
    }

    private void StartSceneFromParam()
    {
        switch(MainMenuParam.param)
        {
            case 0: SceneManager.LoadScene("Menu");break;
            case 1: SceneManager.LoadScene(2); break;
            case 2: SceneManager.LoadScene(3); break;
            case 3: SceneManager.LoadScene("SingleRoom3"); break;
            case 4: SceneManager.LoadScene(5); break;
            case 5: SceneManager.LoadScene("SingleRoom3"); break;
        }
    }
}
