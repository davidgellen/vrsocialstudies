using Dp.Hands;
using Dp.Rig.Bindings;
using Dp.Rig.Interfaces;
using Dp.Rig.Providers;
using UnityEngine;
using ViveHandTracking;

namespace Dp.Rig.Model
{
    [System.Serializable]
    public class RigModel : IRigModel
    {
        [SerializeField] private Transform _cameraRig = null;
        [SerializeField] private Transform _avatarNeck = null;
        [SerializeField] private float _avatarHeight = 0f;
   
        [SerializeField] private HeadBinding _head = null;
        [SerializeField] private HipBinding _hips = null;
        [SerializeField] private HandBinding _leftArm = null;
        [SerializeField] private HandBinding _rightArm = null;
        [SerializeField] private FootBinding _leftLeg = null;
        [SerializeField] private FootBinding _rightLeg = null;

        [SerializeField] private HandData _leftHand = null;
        [SerializeField] private HandData _rightHand = null;

        private IRigDataProvider _rigDataProvider;

        public GestureProvider GetGestureProvider { get; private set; }

        public Transform CameraRig => _cameraRig;

        public float AvatarHeight => _avatarHeight;

        public Transform AvatarNeck => _avatarNeck;

        public HeadBinding Head => _head;

        public HandBinding LeftArm => _leftArm;

        public HandBinding RightArm => _rightArm;

        public HipBinding Hips => _hips;

        public FootBinding LeftLeg => _leftLeg;

        public FootBinding RightLeg => _rightLeg;

        public HandData LeftHand => _leftHand;

        public HandData RightHand => _rightHand;

        public void Initialize() 
        {
            _rigDataProvider = new RigDataProvider(this);
            GetGestureProvider = _cameraRig.GetComponentInChildren<GestureProvider>();

            _head.Initialze(_rigDataProvider);
            _hips.Initialze(_rigDataProvider);
            _leftArm.Initialze(_rigDataProvider);
            _rightArm.Initialze(_rigDataProvider);
            _leftLeg.Initialze(_rigDataProvider);
            _rightLeg.Initialze(_rigDataProvider);
        }
    }
}
