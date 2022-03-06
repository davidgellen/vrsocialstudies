using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private float _lives = 3;
    private Coroutine _startCounter;
    private bool _gameStarted = false;
    private AudioSource _audioSource;
    

    [SerializeField] private Transform[] _walls;
    [SerializeField] private TMP_Text _counterText;
    [SerializeField] private Renderer[] _livesMeshes;
    [SerializeField] private AudioClip _wrongAudioClip;
    [SerializeField] private AudioClip _confirmAudioClip;

    public float GameSpeed = 5f;
    public bool SpawnNextWall = true;
    public int GameState = -1;

    public static GameManager Instance { get { return _instance; } }

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
        _audioSource = GetComponent<AudioSource>();
        ResetLevel();
    }

    private void Update()
    {
        switch (GameState)
        {
            case 0:
                if (_startCounter == null)
                {
                    _startCounter = StartCoroutine(Counter());
                }
                break;
            case 1:
                StartGame();
                break;
            case 2:
                PauseGame();
                break;
        }
    }

    public void StartGame() 
    {
        GameSpeed = 5;
        if (SpawnNextWall) 
        {

            foreach (var item in _walls)
                item.gameObject.SetActive(false);
            _walls[Random.Range(0, _walls.Length - 1)].gameObject.SetActive(true);
            SpawnNextWall = false;
            decreaseLiveCheck = 0;
        }
    }

    public void PauseGame()
    {
        GameSpeed = 0;
        _counterText.text = "";
        if(_startCounter != null)
            StopCoroutine(_startCounter);
        _startCounter = null;
    }


    private int decreaseLiveCheck = 0;
    public void DecreaseLives() 
    {
        decreaseLiveCheck++;
        if (decreaseLiveCheck > 1)
            return;

        _lives--;
        if (_lives <= 0)
            _lives = 0;

        for (int i = 0; i < 3 - _lives; i++)
        {
            _livesMeshes[i].material.color = Color.red;
        }
        _audioSource.clip = _wrongAudioClip;
        _audioSource.Play();
        if (_lives <= 0)
        {
            StartCoroutine(ShowFail());
            return;
        }
        SpawnNextWall = true;
    }

    public IEnumerator Counter()
    {
        int counter = 3;
        while(counter != 0) 
        {
            _counterText.text = counter.ToString();
            yield return new WaitForSeconds(1f);
            counter--;
        }
        _counterText.text = "Start";
        yield return new WaitForSeconds(1f);
        _counterText.text = "";
        _gameStarted = true;
        GameState = 1;
    }

    public void PlayConfirmSound() 
    {
        _audioSource.clip = _confirmAudioClip;
        _audioSource.Play();
    }

    public IEnumerator ShowFail()
    {
        _counterText.text = "<color=red>Konec hry</color>";
        yield return new WaitForSeconds(1f);
        GameState = 4;
        _counterText.text = "";
    }

    public void ResetLevel() 
    {
        _lives = 3;
        _gameStarted = false;
        GameState = -1;
        decreaseLiveCheck = 0;
        SpawnNextWall = true;
        for (int i = 0; i < 3; i++)
        {
            _livesMeshes[i].material.color = Color.gray;
        }
        _counterText.text = "";
        _startCounter = null;
    }
}
