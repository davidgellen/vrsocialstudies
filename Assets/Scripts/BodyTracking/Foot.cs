using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour
{
    [SerializeField] GameObject followObject;
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
        // _body.mass = 20f;
    }

    public void setPositionOffset(Vector3 offset)
    {
        positionOffset = offset;
    }

    void Update()
    {
        PhysicsMove();
    }

    void PhysicsMove()
    {

        _body.transform.position = _followTarget.position + _followTarget.TransformDirection(positionOffset);

        startRotation = transform.rotation;
        endRotation = followObject.transform.rotation;
        transform.rotation = Quaternion.Lerp(startRotation, endRotation, Time.deltaTime * rotationSpeed);
        Vector3 newRotation = new Vector3((followObject.transform.eulerAngles.x + rotationOffset.x), (followObject.transform.eulerAngles.y + rotationOffset.y), (followObject.transform.eulerAngles.z + rotationOffset.z));
        transform.eulerAngles = newRotation;

    }
}
