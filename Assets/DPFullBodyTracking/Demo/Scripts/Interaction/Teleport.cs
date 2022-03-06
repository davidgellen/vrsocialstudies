using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject CameraRig;

    private void Start()
    {
        CameraRig = GameObject.FindGameObjectWithTag("Rig");
    }

    public void OnTeleport() 
    {
        CameraRig.transform.position = transform.position;
        CameraRig.transform.rotation = transform.rotation;
    }
}
