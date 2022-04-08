using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineEnd : MonoBehaviour
{
    void Update()
    {
        if (GetComponent<PlayableDirector>().state != PlayState.Playing)
        {
            SceneManager.LoadScene(1);
        }
    }
}
