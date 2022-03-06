using Dp.Hands;
using UnityEngine;
using UnityEngine.Events;

public class HTCHandTrackingButtonTrigger : ButtonTrigger
{
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OnButtonPressed.AddListener(HandsManager.Instance.EnableHTCTracking);
    }

    private void Update()
    {
        if (HandsManager.Instance.IsHTCHandTrackingEnabled)
        {
            _renderer.material.color = Color.green;
            _renderer.material.SetColor("_EmissionColor", Color.green);
        }
        else
        {
            _renderer.material.color = Color.red;
            _renderer.material.SetColor("_EmissionColor", Color.red);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPressed && other.gameObject.name == "ButtonActivator")
        {
            audioSource.Play();
            IsPressed = true;
            OnButtonPressed?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ButtonActivator")
        {
            IsPressed = false;
        }
    }
}
