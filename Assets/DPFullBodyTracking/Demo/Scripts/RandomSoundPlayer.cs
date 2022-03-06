using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundPlayer : MonoBehaviour
{
    public Color DefaultColor;
    public Color[] ActiveColor;
    public AudioClip[] Sounds;
    public int _collisionEnter = 0;

    private Renderer _renderer;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _audioSource = GetComponent<AudioSource>();

        _renderer.material.color = DefaultColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        _collisionEnter++;
        if (_collisionEnter != 1)
            return;

        int colorIndex = (int)Random.Range(0, ActiveColor.Length);
        int soundIndex = (int)Random.Range(0, Sounds.Length);
        _renderer.material.color = ActiveColor[colorIndex];
        _audioSource.clip = Sounds[soundIndex];
        _audioSource.Play();
    }

    private void OnTriggerExit(Collider collision)
    {
        _collisionEnter--;
        _renderer.material.color = DefaultColor;
    }
}
