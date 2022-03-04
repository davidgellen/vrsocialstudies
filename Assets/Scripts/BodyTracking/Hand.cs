using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] Vector3 rotationOffset;
    [SerializeField] int rotationSpeed;
    private Transform _followTarget;
    private Rigidbody _body;
    [SerializeField] GameObject shoulder;
    [SerializeField] float shoulderSpeed = 1f;

    Quaternion startRotation;
    Quaternion endRotation;

    private float distanceControllerShoulder;

    // Start is called before the first frame update
    void Start()
    {
        _followTarget = followObject.transform;
        _body = GetComponent<Rigidbody>();
        _body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _body.mass = 20f;
    }

    void Update()
    {
        PhysicsMove();
    }

    void PhysicsMove()
    {
        distanceControllerShoulder = _followTarget.position.y - shoulder.transform.position.y;
        if (distanceControllerShoulder > 0) {
            //Quaternion defaultPos = _followTarget.position + _followTarget.TransformDirection(positionOffset);
            //copy.look
            //_body.transform.LookAt(shoulder.transform);
            Vector3 test = Vector3.MoveTowards(_followTarget.position, shoulder.transform.position, shoulderSpeed * distanceControllerShoulder);
            _body.transform.position = test;

            //Vector3 targetDirection = shoulder.transform.position - _body.transform.position;
            //targetDirection = targetDirection.normalized * shoulderSpeed * distanceControllerShoulder * Time.deltaTime;
            //float maxDistance = Vector3.Distance(_body.transform.position, shoulder.transform.position);
            //_body.transform.position = _followTarget.position + Vector3.ClampMagnitude(targetDirection, maxDistance);
        }
        else {
            _body.transform.position = _followTarget.position + _followTarget.TransformDirection(positionOffset);
        }


        startRotation = transform.rotation;
        endRotation = followObject.transform.rotation;
        transform.rotation = Quaternion.Lerp(startRotation, endRotation, Time.deltaTime * rotationSpeed);
        Vector3 newRotation = new Vector3((followObject.transform.eulerAngles.x + rotationOffset.x), (followObject.transform.eulerAngles.y + rotationOffset.y), (followObject.transform.eulerAngles.z + rotationOffset.z));
        transform.eulerAngles = newRotation;

    }
}
