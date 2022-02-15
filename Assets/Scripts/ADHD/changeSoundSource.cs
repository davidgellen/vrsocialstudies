using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;
using System.Threading.Tasks;
using UnityEngine.Windows.Speech;
using CrazyMinnow.SALSA;
using CrazyMinnow.SALSA.OneClicks;

public class changeSoundSource : MonoBehaviour
{

    public GameObject avatar;
    public float time = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    async void Update()
    {
        time = time + Time.deltaTime;

        if (time>5 && time<6){ 
            avatar.GetComponent<Salsa>().audioSrc.Stop();
            string filename = Path.Combine(Application.dataPath, "Scripts", "ADHD", "Tempest.wav");
			/*var www = new WWW(filename);
            Debug.Log(www);
			AudioClip clip = www.GetAudioClip(true, false, AudioType.WAV);*/
            Debug.Log(filename);
            AudioClip clip = await LoadAudioClip(filename);
            if (clip != null){
                avatar.GetComponent<AudioSource>().clip = clip;
                Debug.Log(clip);
                avatar.GetComponent<Salsa>().audioSrc.Play();
            }
            else{
                Debug.Log("je to null");
            }
        }
    }

    async Task<AudioClip> LoadAudioClip(string path){
        AudioClip clip = null;

        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
         uwr.SendWebRequest();
 
         // wrap tasks in try/catch, otherwise it'll fail silently
         try
         {
             while (!uwr.isDone) await Task.Delay(5);
 
             if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
             else
             {
                 clip = DownloadHandlerAudioClip.GetContent(uwr);
             }
         }
         catch (Exception err)
         {
             Debug.Log($"{err.Message}, {err.StackTrace}");
         }
     }
 
     return clip;
    }
}
