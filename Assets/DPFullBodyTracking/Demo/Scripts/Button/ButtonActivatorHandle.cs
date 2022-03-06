using UnityEngine;

public class ButtonActivatorHandle : MonoBehaviour
{
    [SerializeField] private ButtonTrigger _buttonTrigger = null;

    private Vector3 _originPos = Vector3.zero;
    private Vector3 _maxPos = Vector3.zero;
    private float _maxDistance = 0f;
    private float _minDistance = 0f;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(-Physics.gravity * GetComponent<Rigidbody>().mass);

        _minDistance = Vector3.Distance(_buttonTrigger.transform.position, transform.position);
        _maxDistance = _buttonTrigger.transform.position.y;
        _maxPos = new Vector3(transform.position.x, _maxDistance, transform.position.z);
        _originPos = transform.position;
    }

    void Update()
    {
        if (!_buttonTrigger.IsPressed && Vector3.Distance(_buttonTrigger.transform.position, transform.position) >= _minDistance)
            transform.position = _originPos;

        if (transform.position.y <= _maxDistance)
            transform.position = _maxPos;
    }
}
