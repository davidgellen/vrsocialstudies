using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionAndRotation : MonoBehaviour
{
    [SerializeField] GameObject from;
    [SerializeField] GameObject to;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = new Vector3(to.transform.eulerAngles.x, to.transform.eulerAngles.x, to.transform.eulerAngles.z);
        from.transform.eulerAngles = newRotation;
        to.transform.position = from.transform.position;
    }
}
