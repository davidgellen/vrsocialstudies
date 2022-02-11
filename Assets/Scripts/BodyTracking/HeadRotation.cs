using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRotation : MonoBehaviour
{

    [SerializeField] GameObject camera_;
    [SerializeField] GameObject head;
    [SerializeField] float xRotation;
    [SerializeField] float yRotation;
    [SerializeField] float zRotation; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = new Vector3(camera_.transform.eulerAngles.x + xRotation, camera_.transform.eulerAngles.y + yRotation, camera_.transform.eulerAngles.z + zRotation);
        this.transform.eulerAngles = newRotation;

    }
}
