using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : ButtonTrigger
{
    public Scenes LoadScene; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OnButtonPressed.AddListener(() => SceneLoader.Instance.LoadScene(LoadScene));
    }
}
