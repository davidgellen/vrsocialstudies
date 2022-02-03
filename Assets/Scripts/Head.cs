using UnityEngine;

public class Head : MonoBehaviour
{

    [SerializeField] GameObject sourceObject, destinationObject;
    [SerializeField] bool invertXrotation, invertYrotation, invertZrotation;
    [SerializeField] int xRotation, yRotation, zRotation;
    [SerializeField] Vector3 positionOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newRotation = new Vector3(sourceObject.transform.eulerAngles.x + xRotation, sourceObject.transform.eulerAngles.y + yRotation, sourceObject.transform.eulerAngles.z + zRotation);
        destinationObject.transform.eulerAngles = newRotation;
        destinationObject.transform.position = sourceObject.transform.position + sourceObject.transform.TransformDirection(positionOffset);
    }
}
