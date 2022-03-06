using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    private Scenes _currentScene;

    public static SceneLoader Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        _currentScene = Scenes.MainScene;
        if (SceneManager.GetSceneByBuildIndex((int)_currentScene).isLoaded)
            return;
        SceneManager.LoadScene((int)_currentScene, LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) 
        {
            Application.Quit(0);
        }
    }

    public void LoadScene(Scenes scene) 
    {
        SceneManager.UnloadSceneAsync((int)_currentScene);
        _currentScene = scene;
        SceneManager.LoadSceneAsync((int)_currentScene, LoadSceneMode.Additive);
    }
}

