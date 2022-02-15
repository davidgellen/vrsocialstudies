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
    private Transform _followTarget;
    private Rigidbody _body;
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
    void Update()
    {
        PhysicsMove();
    }

    void PhysicsMove()
    {
        // position
        var positionWithOffset = _followTarget.position + positionOffset;
       // var positionWithOffset = _followTarget.TransformPoint(positionOffset);
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        _body.velocity = (positionWithOffset - transform.position).normalized * (followSpeed * distance);

        //rotation
        var rotationWithOffset = _followTarget.rotation * Quaternion.Euler(rotationOffset);
        var q = rotationWithOffset * Quaternion.Inverse(_body.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        //transform.eulerAngles = axis;
        _body.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
    }
}
