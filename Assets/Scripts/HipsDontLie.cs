using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipsDontLie : MonoBehaviour
{
    [SerializeField] GameObject lowerBack;
    [SerializeField] int rotationOffsetX;
    [SerializeField] int rotationOffsetY;
    [SerializeField] int rotationOffsetZ;
    [SerializeField] int positionOffsetX;
    [SerializeField] int positionOffsetY;
    [SerializeField] int positionOffsetZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float time = 1.0f;
    void Update()
    {
        if (time >= 0)
        {
            time -= Time.deltaTime;
            return;
        }
        else
        {
            transform.position = new Vector3(lowerBack.transform.position.x + positionOffsetX, lowerBack.transform.position.y + positionOffsetY, lowerBack.transform.position.z + positionOffsetZ);
        }

    }
}
