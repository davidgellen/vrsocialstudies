using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLimb : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    [SerializeField] float followSpeed = 30f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] Vector3 rotationOffset;
    private Transform _followTarget;
    private Rigidbody _body;
    private float nextActionTime = 0.0f;
    public float period = 0.5f;
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
        //if (axis.x > 360)
        //{
        //    axis.x -= 360;
        //}
        //if (axis.y > 360)
        //{
        //    axis.y -= 360;
        //}
        //if (axis.z > 360)
        //{
        //    axis.z -= 360;
        //}

        //if (axis.x < 0)
        //{
        //    axis.x += 360;
        //}
        //if (axis.y < 0)
        //{
        //    axis.y += 360;
        //}
        //if (axis.z < 0)
        //{
        //    axis.z += 360;
        //}

        var result = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            Debug.Log("x: " + axis.x + ", y: " + axis.y + " , z: " + axis.z);
            Debug.Log("angle: " + angle);
            // execute block of code here
        }


        //if (result.x > 360)
        //{
        //    result.x -= 360;
        //}
        //if (result.y > 360)
        //{
        //    result.y -= 360;
        //}
        //if (result.z > 360)
        //{
        //    result.z -= 360;
        //}

        //if (result.x < 0)
        //{
        //    result.x += 360;
        //}
        //if (result.y < 0)
        //{
        //    result.y += 360;
        //}
        //if (result.z < 0)
        //{
        //    result.z += 360;
        //}

        //_body.angularVelocity = result;
        transform.rotation = Quaternion.Lerp(rotationWithOffset, Quaternion.Euler(result), followSpeed);
    }
}
