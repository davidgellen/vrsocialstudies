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
    [SerializeField] GameObject rightLegController;

    private float expectedHandsDistance = 1.45f;

    private float nextActionTime = 0.0f;
    public float period = 4f;

    // Start is called before the first frame update
    void Start()
    {

        GameObject[] rootBones = { head, hips, leftHand, rightHand, leftLeg, rightLeg };
        GameObject[] targets = { headTarget, hipsTarget, leftHandTarget, rightHandTarget, leftLegTarget, rightLegTarget };
        GameObject[] controllers = { headController, hipsController, leftHandController, rightHandController, leftLegController, rightLegController};
        GameObject[] hints = { leftHandHint, rightHandHint, leftLegHint, rightLegHint };
        alignTargetsToRoots(rootBones, targets);
        StartCoroutine(disableHints(hints));
        StartCoroutine(enableHints(hints));
        StartCoroutine(alighnToHeight());
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.time > nextActionTime)
        //{
        //    nextActionTime += period;
        //    Debug.Log("target Left Hand: " + leftHandTarget.transform.position.y);
        //    Debug.Log("UMA left Hand: " + leftHand.transform.position.y);
        //    Debug.Log("ROZDIEL RUK" + (leftHandTarget.transform.position.y - leftHand.transform.position.y));
        //}
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
        yield return new WaitForSeconds(4);
        GameObject[] rootBones = { head, hips, leftHand, rightHand, leftLeg, rightLeg };
        GameObject[] targets = { headTarget, hipsTarget, leftHandTarget, rightHandTarget, leftLegTarget, rightLegTarget };
        GameObject[] controllers = { headController, hipsController, leftHandController, rightHandController, leftLegController, rightLegController };

        float distanceRightLegUmaHips = Vector3.Distance(rightLeg.transform.position, hips.transform.position);
        float distanceRightLegHipsController = Vector3.Distance(rightLegController.transform.position, hips.transform.position);
        float diff = distanceRightLegHipsController - distanceRightLegUmaHips;
        rightLegTarget.GetComponent<Foot>().setPositionOffset(new Vector3(diff, 0, 0));
        leftLegTarget.GetComponent<Foot>().setPositionOffset(new Vector3(diff, 0, 0));

        float distanceRightLegHead = Vector3.Distance(rightLeg.transform.position, head.transform.Find("HeadTop").gameObject.transform.position);
        float distanceRightLegHeadController = Vector3.Distance(rightLegController.transform.position, head.transform.Find("HeadTop").gameObject.transform.position);
        float diffHead = distanceRightLegHeadController - distanceRightLegHead;
        headController.transform.parent.gameObject.transform.position = new Vector3(headController.transform.parent.gameObject.transform.position.x, headController.transform.parent.gameObject.transform.position.y - diffHead, headController.transform.parent.gameObject.transform.position.z);

        transform.position = transform.position - new Vector3(0, diff, 0);
       // hipsTarget.GetComponent<CopyTransform>().setPositionOffset(hipsTarget.GetComponent<CopyTransform>().getPositionOffest() - new Vector3(diff, 0, 0));
        //Debug.Log("distanceRightLegHead: " + distanceRightLegHead);
        //Debug.Log("distanceRightLegHeadController: " + distanceRightLegHeadController);
        //Debug.Log("diff: " + diffHead);
    }

    IEnumerator enableHints(GameObject[] hints)
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < hints.Length; i++)
        {
            hints[i].GetComponent<HintMover>().enabled = true;
        }
        Debug.Log("enabled Hints");
    }

    IEnumerator disableHints(GameObject[] hints)
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < hints.Length; i++)
        {
            hints[i].GetComponent<HintMover>().enabled = false;
        }
        Debug.Log("disabled Hints");
    }
}
