using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmbodymentButton : MonoBehaviour
{
    public float time = 0;
    public float timeLimit = 30;
    public TextMesh timeText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time = time + Time.deltaTime;
        
        // show time
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (time > timeLimit)
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
            case 1: SceneManager.LoadScene(2); break;
            case 2: SceneManager.LoadScene(3); break;
            case 3: SceneManager.LoadScene("SingleRoom2"); break;
            case 4: SceneManager.LoadScene(5); break;
            case 5: SceneManager.LoadScene("SingleRoom2"); break;
        }
    }
}
