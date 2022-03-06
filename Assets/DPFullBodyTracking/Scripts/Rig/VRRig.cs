using UnityEngine;
using Dp.Rig.Model;
using System.Linq;

namespace Dp.Rig
{
    public class VRRig : MonoBehaviour
    {
        private Vector3[] _bonesStartPosition;
        private Quaternion[] _bonesStartRotation;

        // used in rig generator
        public ConstrainedBonesData BonesData;

        public RigModel Model;
        public bool Calibrated = false;
        public Animator Anim;

        public Quaternion LeftHandPalmStartRotation { get; private set; }
        public Quaternion[] LeftThumbRotations { get; private set; }
        public Quaternion RightHandPalmStartRotation { get; private set; }
        public Quaternion[] RightThumbRotations { get; private set; }

        private void OnDisable()
        {
            Calibrated = false;
            Anim.enabled = false;
            Model.Head.IsInitialized = false;
            Model.Hips.IsInitialized = false;
            Model.LeftLeg.IsInitialized = false;
            Model.RightLeg.IsInitialized = false;
            Model.LeftArm.IsInitialized = false;
            Model.RightArm.IsInitialized = false;

            var transforms = GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
                transforms[i].SetPositionAndRotation(_bonesStartPosition[i], _bonesStartRotation[i]);
        }

        private void OnEnable()
        {
            if (_bonesStartPosition == null)
                StoreDefaultPosition();
        }

        public void Start()
        {
            if (Anim == null)
            {
                Anim = GetComponent<Animator>();
                Anim.enabled = false;
            }
        }

        public void FixedUpdate()
        {
            if (!Calibrated)
                return;

            Model.Head.TryBind();
            Model.Hips.TryBind();
            Model.LeftLeg.TryBind();
            Model.RightLeg.TryBind();
            Model.LeftArm.TryBind();
            Model.RightArm.TryBind();
        }

        public void AlignAvatar() 
        {
            var alignPos = new Vector3(
                   Model.Head.Source.parent.position.x,
                   Model.CameraRig.position.y,
                   Model.Head.Source.parent.position.z);

            var alignRot = Camera.main.transform.rotation.eulerAngles.y * Vector3.up;

            transform.parent.position = alignPos;
            transform.parent.rotation = Quaternion.Euler(alignRot);
        }

        public void UpdateScale()
        {
            if (Anim == null)
                Anim = GetComponent<Animator>();
            Anim.enabled = true;
            Camera.main.transform.parent.parent.localScale = (Model.AvatarHeight / Model.Head.Source.parent.position.y) * Vector3.one;
            //Model.Hips.Source.parent.parent.localScale = (Model.Hips.Target.position.y / Model.Hips.Source.parent.position.y) * Vector3.one;
        }

        public void StoreDefaultPosition()
        {
            if (_bonesStartPosition != null)
                return;

            var transforms = GetComponentsInChildren<Transform>(true);
            _bonesStartPosition = transforms.Select(item => item.position).ToArray();
            _bonesStartRotation = transforms.Select(item => item.rotation).ToArray();

            LeftHandPalmStartRotation = Model.LeftHand.Palm.rotation;
            LeftThumbRotations = StoreThumbStartRotation(Model.LeftHand.Thumb);
            RightHandPalmStartRotation = Model.RightHand.Palm.rotation;
            RightThumbRotations = StoreThumbStartRotation(Model.RightHand.Thumb);
        }

        public Quaternion[] StoreThumbStartRotation(Transform thumb) 
        {
            var rotations = new Quaternion[3];
            var transforms = thumb.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < rotations.Length; i++)
            {
                rotations[i] = transforms[i].localRotation;
            }

            return rotations;
        }
    }
}
