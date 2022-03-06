using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VelocitySound : MonoBehaviour
{
    private AudioSource _source;
    private Rigidbody rb;
    private float maxVelocity = 0;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float offenderSensitivity = 10f;
    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        maxVelocity = Mathf.Max(maxVelocity, Mathf.Abs(rb.velocity.magnitude));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && maxVelocity > sensitivity) 
        {
            maxVelocity = 0;
            _source.Play();
        }

        if (collision != null)
        {
            var otherRb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                return;
            if (rb.velocity.magnitude > offenderSensitivity)
            {
                _source.Play();
            }
        }
    }
}
