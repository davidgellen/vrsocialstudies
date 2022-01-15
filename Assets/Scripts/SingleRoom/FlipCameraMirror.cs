using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCameraMirror : MonoBehaviour
{
    [SerializeField] Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPreCull() {
         camera.ResetWorldToCameraMatrix();
         camera.ResetProjectionMatrix();
         Vector3 scale = new Vector3(-1, 1, 1);
         camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(scale);
     }
}
