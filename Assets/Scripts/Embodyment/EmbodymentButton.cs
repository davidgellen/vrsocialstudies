using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmbodymentButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
