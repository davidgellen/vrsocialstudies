using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    [SerializeField] float followSpeed = 30f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] Vector3 rotationOffset;
    [SerializeField] int rotationSpeed;
    private Transform _followTarget;
    private Rigidbody _body;

    float rotationProgress = -1;
    Quaternion startRotation;
    Quaternion endRotation;

    // Start is called before the first frame update
    void Start()
    {
        _followTarget = followObject.transform;
        _body = GetComponent<Rigidbody>();
        _body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _body.mass = 20f;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    PhysicsMove();
    //}

    //void PhysicsMove()
    //{
    //    position
    //   var positionWithOffset = _followTarget.position + positionOffset;
    //    var distance = Vector3.Distance(positionWithOffset, transform.position);
    //    _body.velocity = (positionWithOffset - transform.position).normalized * (followSpeed * distance);

    //    _body.transform.position = _followTarget.position + _followTarget.TransformDirection(positionOffset);

    //    startRotation = transform.rotation;
    //    endRotation = followObject.transform.rotation;
    //    transform.rotation = Quaternion.Lerp(startRotation, endRotation, Time.deltaTime * rotationSpeed);
    //    Vector3 newRotation = new Vector3((followObject.transform.eulerAngles.x + rotationOffset.x), (followObject.transform.eulerAngles.y + rotationOffset.y), (followObject.transform.eulerAngles.z + rotationOffset.z));
    //    transform.eulerAngles = newRotation;

    //    rotation
    //    var rotationWithOffset = _followTarget.rotation * Quaternion.Euler(rotationOffset);
    //    var q = rotationWithOffset * Quaternion.Inverse(_body.rotation);
    //    q.ToAngleAxis(out float angle, out Vector3 axis);
    //    transform.eulerAngles = axis;
    //    _body.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
    //}

    void StartRotating()
    {
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(followObject.transform.rotation.eulerAngles.x, followObject.transform.rotation.eulerAngles.y, followObject.transform.rotation.eulerAngles.z);
        rotationProgress = 0;
    }

    private void Update()
    {
        _body.transform.position = _followTarget.position + _followTarget.TransformDirection(positionOffset);

        Vector3 direction = new Vector3(followObject.transform.rotation.eulerAngles.x, followObject.transform.rotation.eulerAngles.y, followObject.transform.rotation.eulerAngles.z);
        Quaternion targetRotation = Quaternion.Euler(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
