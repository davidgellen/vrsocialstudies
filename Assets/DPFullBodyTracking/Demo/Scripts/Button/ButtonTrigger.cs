using UnityEngine;
using UnityEngine.Events;

public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] protected Renderer _renderer = null;
    [SerializeField] protected UnityEvent OnButtonPressed = null;
    protected AudioSource audioSource;
    public bool IsPressed { get; set; } = false;
    

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPressed && other.gameObject.name == "ButtonActivator")
        {
            audioSource.Play();
            IsPressed = true;
            OnButtonPressed?.Invoke();
            _renderer.material.color = Color.green;
            _renderer.material.SetColor("_EmissionColor", Color.green);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ButtonActivator")
        {
            IsPressed = false;
            _renderer.material.color = Color.red;
            _renderer.material.SetColor("_EmissionColor", Color.red);
        }
    }

    public void ShowObject(GameObject obj) 
    {
        bool enable = obj.activeSelf ? false : true;
        obj.SetActive(enable);
    }
}
