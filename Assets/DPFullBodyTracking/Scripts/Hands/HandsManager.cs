using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveHandTracking;

namespace Dp.Hands
{
    public class HandsManager : MonoBehaviour
    {
        private static HandsManager _instance;
        public static HandsManager Instance { get { return _instance; } }


        public bool IsHTCHandTrackingEnabled = false;
        public bool IsAnyHand => IsAnyHandVisible();

        public Transform[] HTCHands;

        private GestureProvider _htcGestureProvider;


        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        void Start()
        {
            _htcGestureProvider = Camera.main.GetComponent<GestureProvider>();

            if (IsHTCHandTrackingEnabled)
                EnableHTCTracking();
        }

        private bool IsAnyHandVisible()
        {
            bool isHandVisible = false;
            if (GestureProvider.LeftHand != null || GestureProvider.RightHand != null)
                isHandVisible = true;

            return isHandVisible;
        }

        public bool IsHandVisible(bool IsLeft = false)
        {
            bool isHandVisible = false;
            isHandVisible = IsLeft ? GestureProvider.LeftHand != null : GestureProvider.RightHand != null;

            return isHandVisible;
        }

        public void EnableHTCTracking() 
        {
            IsHTCHandTrackingEnabled = true;
            _htcGestureProvider.enabled = true;

            SetActiveHands(HTCHands, true);
        }


        public void SetActiveHands(Transform[] hands, bool active) 
        {
            foreach (var item in hands)
            {
                item.gameObject.SetActive(active);
                if (!active)
                    item.transform.position = Vector3.zero;
            }
        }
    }
}