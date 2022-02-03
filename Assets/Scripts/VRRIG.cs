using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarger;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarger.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarger.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRRIG : MonoBehaviour
{
    public Transform headConstraint;
    public Vector3 headBodyOffset;

    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - headConstraint.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = headConstraint.position + headBodyOffset;
        //transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;
    }

    private void LateUpdate()
    {
        //transform.position = headConstraint.position + headBodyOffset;
        //transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;

        head.Map();
        //leftHand.Map();
        //rightHand.Map();
    }
}
