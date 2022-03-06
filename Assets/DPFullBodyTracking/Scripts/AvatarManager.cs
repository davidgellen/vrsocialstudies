using UnityEngine;
using Dp.Rig;
using ViveHandTracking;

namespace Dp
{
    public class AvatarManager : MonoBehaviour
    {
        private static AvatarManager _instance;
        private GameObject activeAvatar = null;

        public Transform Rig;
        public GameObject[] AvatarModels;
        public GameObject[] RigedAvatars;
        public GameObject[] DefaultControllers;
        public int SelectedAvatar = -1;

        public static AvatarManager Instance { get { return _instance; } }

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

        public void AvatarSelected(Terminal terminal)
        {
            if (activeAvatar != null)
                activeAvatar.SetActive(false);

            activeAvatar = RigedAvatars[SelectedAvatar];
            activeAvatar.SetActive(true);
            StartCoroutine(Calibration.StartCalibration(activeAvatar.GetComponentInChildren<VRRig>(true), terminal, DefaultControllers));
        }

        public void OnLikeGestureStateChanged(int state)
        {
            if (state == 2)
                Calibration.Continue = true;
        }

        public void LeapMotionLikeGestureDetected() 
        {
            Calibration.Continue = true;
        }
    }
}
