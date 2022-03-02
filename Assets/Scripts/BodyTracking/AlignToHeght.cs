using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToHeght : MonoBehaviour
{
    [SerializeField] GameObject head;
    [SerializeField] GameObject headTarget;
    [SerializeField] GameObject hips;
    [SerializeField] GameObject hipsTarget;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject leftHandTarget;
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject rightHandTarget;
    [SerializeField] GameObject leftLeg;
    [SerializeField] GameObject leftLegTarget;
    [SerializeField] GameObject rightLeg;
    [SerializeField] GameObject rightLegTarget;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] rootBones = { head, hips, leftHand, rightHand, leftLeg, rightLeg };
        GameObject[] targets = { headTarget, hipsTarget, leftHandTarget, rightHandTarget, leftLegTarget, rightLegTarget };
        alignTargetsToRoots(rootBones, targets);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void alignTargetsToRoots(GameObject[] rootBones, GameObject[] targets)
    {
        for (int i = 0; i < rootBones.Length; i++)
        {
            Vector3 newRotation = new Vector3(rootBones[i].transform.eulerAngles.x, rootBones[i].transform.eulerAngles.y, rootBones[i].transform.eulerAngles.z);
            Vector3 newPosition = new Vector3(rootBones[i].transform.position.x, rootBones[i].transform.position.y, rootBones[i].transform.position.z);
            targets[i].transform.eulerAngles = newRotation;
            targets[i].transform.position = newPosition;
        }
    }
}
