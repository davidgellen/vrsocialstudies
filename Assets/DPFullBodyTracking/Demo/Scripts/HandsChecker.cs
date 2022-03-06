using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveHandTracking;

public class HandsChecker : MonoBehaviour
{
    public GameObject LeftHand;
    public GameObject RightHand;

    private void Start()
    {
        LeftHand.SetActive(false);
        RightHand.SetActive(false);
    }

    void Update()
    {
        if (GestureProvider.Status != GestureStatus.Running ||!GestureProvider.Current.enabled)
            return;

        if (GestureProvider.LeftHand != null) LeftHand.SetActive(true); else LeftHand.SetActive(false);
        if (GestureProvider.RightHand != null) RightHand.SetActive(true); else RightHand.SetActive(false);
    }
}
