using UnityEngine;

public class Head : MonoBehaviour
{

    [SerializeField] GameObject cameraObj, target;
    [SerializeField] GameObject characterHead;
    [SerializeField] bool invertXrotation, invertYrotation, invertZrotation;
    [SerializeField] int xRotation, yRotation, zRotation;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] float rotationCoeficient = 0.005f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        //    Vector3 newRotation = new Vector3(cameraObj.transform.eulerAngles.x + xRotation, cameraObj.transform.eulerAngles.y + yRotation, cameraObj.transform.eulerAngles.z + zRotation);
        //    target.transform.eulerAngles = newRotation;
        //    target.transform.position = cameraObj.transform.position + cameraObj.transform.TransformDirection(positionOffset);
        Vector3 cameraRotation = new Vector3(cameraObj.transform.eulerAngles.x, cameraObj.transform.eulerAngles.y, cameraObj.transform.eulerAngles.z);
        if (cameraRotation.x > 0 && cameraRotation.x < 90)
        {
            positionOffset.y = -cameraRotation.x * rotationCoeficient;
        }
        target.transform.eulerAngles = new Vector3(cameraRotation.x + xRotation, cameraRotation.y + yRotation, cameraRotation.z + zRotation);

        Vector3 newPosition = cameraObj.transform.position + cameraObj.transform.TransformDirection(positionOffset);
        target.transform.position = newPosition;
    }
}
