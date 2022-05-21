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
    [SerializeField] bool calibrated = false;
    [SerializeField] float globalHeightThreshold = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _followTarget = followObject.transform;
        _body = GetComponent<Rigidbody>();
        _body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _body.interpolation = RigidbodyInterpolation.None;
        _body.mass = 20f;
    }

    public void setLocalPositionOffset(Vector3 offset)
    {
        positionOffset = offset;
    }

    public void setGlobalHeightThreshold(float newThreshold)
    {
        globalHeightThreshold = newThreshold;
        calibrated = true;
    }


    void Update()
    {
        PhysicsMove();
    }

    void PhysicsMove()
    {
        Vector3 wantedPosition = _followTarget.position + _followTarget.TransformDirection(positionOffset);
        if (calibrated)
        {
            wantedPosition.y = Mathf.Max(wantedPosition.y, globalHeightThreshold * 0.85f);
        }
        _body.transform.position = wantedPosition;

        startRotation = transform.rotation;
        endRotation = followObject.transform.rotation;
        _body.transform.rotation = Quaternion.Lerp(startRotation, endRotation, Time.deltaTime * rotationSpeed);
        Vector3 newRotation = new Vector3((followObject.transform.eulerAngles.x + rotationOffset.x), (followObject.transform.eulerAngles.y + rotationOffset.y), (followObject.transform.eulerAngles.z + rotationOffset.z));
        _body.transform.eulerAngles = newRotation;

    }
}
