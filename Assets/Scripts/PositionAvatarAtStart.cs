using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAvatarAtStart : MonoBehaviour

{
    [SerializeField] GameObject camera_;
    [SerializeField] float xOffset;
    [SerializeField] float yxOffset;
    [SerializeField] float zOffset;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(camera_.transform.position.x + xOffset , xOffset, camera_.transform.position.z + zOffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
