using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] public float rotateDegree;  
    [SerializeField] public float rotateInterval;     
    [SerializeField] private float rotateTime = 1f;      
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RotateObject(Vector3.right * rotateDegree, rotateTime));
    }

    IEnumerator RotateObject(Vector3 byAngles, float inTime) { 
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        while (true) {
            yield return new WaitForSeconds(rotateInterval);
            for (var t = 0f; t <= 1; t += Time.deltaTime/inTime) {
                transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
                yield return null;
            }
            fromAngle = transform.rotation;
            toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
