using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToHeght : MonoBehaviour
{
    [SerializeField] GameObject head;
    [SerializeField] GameObject headTarget;
    [SerializeField] GameObject headController;
    [SerializeField] GameObject hips;
    [SerializeField] GameObject hipsTarget;
    [SerializeField] GameObject hipsController;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject leftHandTarget;
    [SerializeField] GameObject leftHandHint;
    [SerializeField] GameObject leftHandController;
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject rightHandTarget;
    [SerializeField] GameObject rightHandHint;
    [SerializeField] GameObject rightHandController;
    [SerializeField] GameObject leftLeg;
    [SerializeField] GameObject leftLegTarget;
    [SerializeField] GameObject leftLegHint;
    [SerializeField] GameObject leftLegController;
    [SerializeField] GameObject rightLeg;
    [SerializeField] GameObject rightLegTarget;
    [SerializeField] GameObject rightLegHint;
    [SerializeField] GameObject rightlegController;

    // Start is called before the first frame update
    void Start()
    {

        GameObject[] rootBones = { head, hips, leftHand, rightHand, leftLeg, rightLeg };
        GameObject[] targets = { headTarget, hipsTarget, leftHandTarget, rightHandTarget, leftLegTarget, rightLegTarget };
        GameObject[] controllers = { headController, hipsController, leftHandController, rightHandController, leftLegController, rightlegController};
        GameObject[] hints = { leftHandHint, rightHandHint, leftLegHint, rightLegHint };
        alignTargetsToRoots(rootBones, targets);
        StartCoroutine(alighnToHeight());
        StartCoroutine(disableHints(hints));
        StartCoroutine(enableHints(hints));
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

    IEnumerator alighnToHeight()
    {
        yield return new WaitForSeconds(2);
        GameObject[] rootBones = { head, hips, leftHand, rightHand, leftLeg, rightLeg };
        GameObject[] targets = { headTarget, hipsTarget, leftHandTarget, rightHandTarget, leftLegTarget, rightLegTarget };
        GameObject[] controllers = { headController, hipsController, leftHandController, rightHandController, leftLegController, rightlegController };
        float distanceHands = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
        float distanceHandControllers = Vector3.Distance(leftHandController.transform.position, rightHandController.transform.position);
        Debug.Log("distanceHands: " + distanceHands);
        Debug.Log("distanceHandControllers: " + distanceHandControllers);

        float distanceRightLegHead = Vector3.Distance(rightLeg.transform.position, head.transform.position);
        float distanceRightLegHeadController = Vector3.Distance(rightlegController.transform.position, head.transform.position);
        Debug.Log("distanceRightLegHead: " + distanceRightLegHead);
        Debug.Log("distanceRightLegHeadController: " + distanceRightLegHeadController);
    }

    IEnumerator enableHints(GameObject[] hints)
    {
        yield return new WaitForSeconds(4);
        for (int i = 0; i < hints.Length; i++)
        {
            hints[i].GetComponent<HintMover>().enabled = true;
        }
        Debug.Log("enabled Hints");
    }

    IEnumerator disableHints(GameObject[] hints)
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < hints.Length; i++)
        {
            hints[i].GetComponent<HintMover>().enabled = false;
        }
        Debug.Log("disabled Hints");
    }
}
