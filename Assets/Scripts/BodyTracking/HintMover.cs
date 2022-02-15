using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintMover : MonoBehaviour
{
    [SerializeField] public GameObject target;
    [SerializeField] public GameObject hint;
    [SerializeField] public Vector3 positionOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hint.transform.position = target.transform.position + target.transform.TransformDirection(positionOffset);
        hint.transform.eulerAngles = new Vector3(target.transform.eulerAngles.x, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
    }
}
