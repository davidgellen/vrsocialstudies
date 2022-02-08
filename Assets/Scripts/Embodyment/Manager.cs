using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] GameObject mirrorFrame;
    [SerializeField] GameObject mirrorCamera;
    [SerializeField] float embodymentTime = 10;
    [SerializeField] TextMesh timeText;

    private bool mirrorShowing = false;
    private bool timerIsRunning = true;
    

    void Start()
    {
        mirrorShowing = false;
        timerIsRunning = true;
    }

   
    void Update()
    {
        flickMirrorState();
        updateTimer();
    }

    void flickMirrorState(){
        if (Input.GetKeyUp(KeyCode.A)) {
            mirrorShowing = !mirrorShowing;
            mirrorFrame.GetComponent<MeshRenderer>().enabled = mirrorShowing;
            mirrorCamera.GetComponent<MeshRenderer>().enabled = mirrorShowing;
        }
    }

    void updateTimer(){
        if (timerIsRunning) {
            if (embodymentTime > 0) {
                embodymentTime -= Time.deltaTime;
                DisplayTime(embodymentTime);
            }
            else {
                Debug.Log("Time has run out!");
                embodymentTime = 0;
                timerIsRunning = false;
            }
        }
    }

    void DisplayTime(float timeToDisplay) {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
