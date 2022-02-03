using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    [SerializeField] GameObject sourceObject;
    [SerializeField] GameObject destinationObject;
    [SerializeField] float rotationOffsetX;
    [SerializeField] float rotationOffsetY;
    [SerializeField] float rotationOffsetZ;
    [SerializeField] float positionOffsetX;
    [SerializeField] float positionOffsetY;
    [SerializeField] float positionOffsetZ;
    [SerializeField] bool invertXrotation = false;
    [SerializeField] bool invertYrotation = false;
    [SerializeField] bool invertZrotation = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = new Vector3((sourceObject.transform.eulerAngles.x + rotationOffsetX) * (invertXrotation ? -1 : 1), (sourceObject.transform.eulerAngles.y + rotationOffsetY) * (invertYrotation ? -1 : 1), (sourceObject.transform.eulerAngles.z + rotationOffsetZ) * (invertZrotation ? -1 : 1));
        Vector3 newPosition = new Vector3(sourceObject.transform.position.x + positionOffsetX, sourceObject.transform.position.y + positionOffsetY, sourceObject.transform.position.z + positionOffsetZ);
        destinationObject.transform.eulerAngles = newRotation;
        destinationObject.transform.position = newPosition;
    }
}
