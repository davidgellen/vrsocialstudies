using Dp.ActiveRagdoll.Data;
using Dp.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dp.ActiveRagdoll
{
    public class ActiveRagdoll : MonoBehaviour
    {
        [Header("Collisions settings")]
        [SerializeField] private LayerMask _CollisionMask;
        [SerializeField] private float _forceRatioGain = 0.1f;
        [SerializeField] private float _minimumForceRatio = 0.05f;
        [SerializeField] private float _maximumForceRatio = 1f;
        [SerializeField] private AnimationFollow _animFollow;
        [SerializeField] private float currentForceRatio = 1f;
        [SerializeField] private JointLimitValues _jointLimitValues;
        private Dictionary<HumanBodyBones, Bone> _bonesDict;
        private bool IsReady = false;

        public BoneSetup SlaveBones;
        [Header("Ragdoll settings")] public Transform MasterRoot;
        public bool EnableLimits;
        public Axis JointDirection = Axis.Y;

        public int CollisionDetected { get; set; } = 0;
        public Dictionary<HumanBodyBones, Bone> BonesDict => _bonesDict;
        public LayerMask CollisionMask => _CollisionMask;

        private void OnEnable()
        {
            CollisionDetected = 0;
            if (_animFollow != null)
                _animFollow.ForceCoeficient = 1f;
        }

        void Start()
        {
            InitBones();
            if(EnableLimits)
                SetLimits();
            _animFollow.InitializeModel(this);
            IsReady = true;
        }

        void FixedUpdate()
        {
            if (!IsReady)
                return;

            _animFollow.FollowAnimation();

            if (CollisionDetected > 0)
                LooseForce();
            else 
                GainForce();
        }

        private void InitBones() 
        {
            _bonesDict = new Dictionary<HumanBodyBones, Bone>();
            MapBones();
        }

        // due to the random position in the hierarchy, each bone must be set separatly
        private void MapBones() 
        {
            //hips
            _bonesDict.Add(HumanBodyBones.Hips, GetBone(SlaveBones.Hips, true));

            _bonesDict.Add(HumanBodyBones.LeftUpperLeg, GetBone(SlaveBones.LeftFoot.parent.parent));
            _bonesDict.Add(HumanBodyBones.LeftLowerLeg, GetBone(SlaveBones.LeftFoot.parent));
            _bonesDict.Add(HumanBodyBones.LeftFoot, GetBone(SlaveBones.LeftFoot));
            _bonesDict.Add(HumanBodyBones.LeftToes, GetBone(SlaveBones.LeftFoot.GetChild(0)));

            _bonesDict.Add(HumanBodyBones.RightUpperLeg, GetBone(SlaveBones.RightFoot.parent.parent));
            _bonesDict.Add(HumanBodyBones.RightLowerLeg, GetBone(SlaveBones.RightFoot.parent));
            _bonesDict.Add(HumanBodyBones.RightFoot, GetBone(SlaveBones.RightFoot));
            _bonesDict.Add(HumanBodyBones.RightToes, GetBone(SlaveBones.RightFoot.GetChild(0)));

            _bonesDict.Add(HumanBodyBones.Spine, GetBone(SlaveBones.Head.parent.parent.parent.parent));
            _bonesDict.Add(HumanBodyBones.Chest, GetBone(SlaveBones.Head.parent.parent.parent));
            _bonesDict.Add(HumanBodyBones.UpperChest, GetBone(SlaveBones.Head.parent.parent));
            _bonesDict.Add(HumanBodyBones.Neck, GetBone(SlaveBones.Head.parent));
            _bonesDict.Add(HumanBodyBones.Head, GetBone(SlaveBones.Head));

            _bonesDict.Add(HumanBodyBones.LeftShoulder, GetBone(SlaveBones.LeftHand.parent.parent.parent));            
            _bonesDict.Add(HumanBodyBones.LeftUpperArm, GetBone(SlaveBones.LeftHand.parent.parent));
            _bonesDict.Add(HumanBodyBones.LeftLowerArm, GetBone(SlaveBones.LeftHand.parent));
            _bonesDict.Add(HumanBodyBones.LeftHand, GetBone(SlaveBones.LeftHand));

            _bonesDict.Add(HumanBodyBones.RightShoulder, GetBone(SlaveBones.RightHand.parent.parent.parent));
            _bonesDict.Add(HumanBodyBones.RightUpperArm, GetBone(SlaveBones.RightHand.parent.parent));
            _bonesDict.Add(HumanBodyBones.RightLowerArm, GetBone(SlaveBones.RightHand.parent));
            _bonesDict.Add(HumanBodyBones.RightHand, GetBone(SlaveBones.RightHand));

            _bonesDict.Add(HumanBodyBones.LeftThumbProximal, GetBone(SlaveBones.LeftThumb));
            _bonesDict.Add(HumanBodyBones.LeftThumbIntermediate, GetBone(SlaveBones.LeftThumb.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftThumbDistal, GetBone(SlaveBones.LeftThumb.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftIndexProximal, GetBone(SlaveBones.LeftIndex));
            _bonesDict.Add(HumanBodyBones.LeftIndexIntermediate, GetBone(SlaveBones.LeftIndex.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftIndexDistal, GetBone(SlaveBones.LeftIndex.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftMiddleProximal, GetBone(SlaveBones.LeftMiddle));
            _bonesDict.Add(HumanBodyBones.LeftMiddleIntermediate, GetBone(SlaveBones.LeftMiddle.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftMiddleDistal, GetBone(SlaveBones.LeftMiddle.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftRingProximal, GetBone(SlaveBones.LeftRing));
            _bonesDict.Add(HumanBodyBones.LeftRingIntermediate, GetBone(SlaveBones.LeftRing.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftRingDistal, GetBone(SlaveBones.LeftRing.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftLittleProximal, GetBone(SlaveBones.LeftPinky));
            _bonesDict.Add(HumanBodyBones.LeftLittleIntermediate, GetBone(SlaveBones.LeftPinky.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.LeftLittleDistal, GetBone(SlaveBones.LeftPinky.GetChild(0).GetChild(0)));

            _bonesDict.Add(HumanBodyBones.RightThumbProximal, GetBone(SlaveBones.RightThumb));
            _bonesDict.Add(HumanBodyBones.RightThumbIntermediate, GetBone(SlaveBones.RightThumb.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightThumbDistal, GetBone(SlaveBones.RightThumb.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightIndexProximal, GetBone(SlaveBones.RightIndex));
            _bonesDict.Add(HumanBodyBones.RightIndexIntermediate, GetBone(SlaveBones.RightIndex.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightIndexDistal, GetBone(SlaveBones.RightIndex.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightMiddleProximal, GetBone(SlaveBones.RightMiddle));
            _bonesDict.Add(HumanBodyBones.RightMiddleIntermediate, GetBone(SlaveBones.RightMiddle.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightMiddleDistal, GetBone(SlaveBones.RightMiddle.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightRingProximal, GetBone(SlaveBones.RightRing));
            _bonesDict.Add(HumanBodyBones.RightRingIntermediate, GetBone(SlaveBones.RightRing.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightRingDistal, GetBone(SlaveBones.RightRing.GetChild(0).GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightLittleProximal, GetBone(SlaveBones.RightPinky));
            _bonesDict.Add(HumanBodyBones.RightLittleIntermediate, GetBone(SlaveBones.RightPinky.GetChild(0)));
            _bonesDict.Add(HumanBodyBones.RightLittleDistal, GetBone(SlaveBones.RightPinky.GetChild(0).GetChild(0)));

        }

        private Bone GetBone(Transform slaveBone, bool root = false) 
        {
            Transform masterBone = MasterRoot.GetComponentsInChildren<Transform>().Where(item => item.name == slaveBone.name).FirstOrDefault();
            return new Bone(slaveBone, masterBone, root);
        }

        private void SetLimits()
        {
            SetLegsLimits(BonesDictionary.LeftLegBones.Concat(BonesDictionary.RightLegBones).ToArray());
            SetFingersLimits(BonesDictionary.LeftHandBones.Concat(BonesDictionary.RightHandBones).ToArray());
            SetArmsLimits(BonesDictionary.LeftArmBones.Concat(BonesDictionary.RightArmBones).ToArray());
        }

        private void SetLegsLimits(HumanBodyBones[] bones) 
        {
            foreach (var key in bones)
            {
                var bone = BonesDict[key];
                switch (key) 
                {
                    case HumanBodyBones.LeftUpperLeg:
                    case HumanBodyBones.RightUpperLeg:
                        SetLimits(bone.Joint, _jointLimitValues.UpperLeg);
                        break;

                    case HumanBodyBones.LeftLowerLeg:
                    case HumanBodyBones.RightLowerLeg:
                        SetLimits(bone.Joint, _jointLimitValues.LowerLeg);
                        break;

                    case HumanBodyBones.LeftFoot:
                    case HumanBodyBones.RightFoot:
                        SetLimits(bone.Joint, _jointLimitValues.Foot);
                        break;

                    case HumanBodyBones.LeftToes:
                    case HumanBodyBones.RightToes:
                        SetLimits(bone.Joint, _jointLimitValues.Toes);
                        break;
                }
            }
        }

        private void SetArmsLimits(HumanBodyBones[] bones)
        {          
            foreach (var key in bones)
            {
                var bone = BonesDict[key];
                switch (key)
                {
                    case HumanBodyBones.LeftShoulder:
                    case HumanBodyBones.RightShoulder:
                        SetLimits(bone.Joint, _jointLimitValues.Shoulder);
                        break;

                    case HumanBodyBones.LeftLowerArm:
                    case HumanBodyBones.RightLowerArm:
                        SetLimits(bone.Joint, _jointLimitValues.LowerArm);
                        break;
                }
            }
        }

        private void SetFingersLimits(HumanBodyBones[] bones)
        {
            foreach (var key in bones)
            {
                var bone = BonesDict[key];
                switch (key) 
                {
                    case HumanBodyBones.RightThumbProximal:
                    case HumanBodyBones.LeftThumbProximal:
                        SetLimits(bone.Joint, _jointLimitValues.FingerThumbProximal);
                        break;
                    case HumanBodyBones.LeftThumbDistal:
                    case HumanBodyBones.LeftThumbIntermediate:
                    case HumanBodyBones.RightThumbDistal:
                    case HumanBodyBones.RightThumbIntermediate:
                        SetLimits(bone.Joint, _jointLimitValues.FingerThumb);
                        break;
                    case HumanBodyBones.LeftIndexProximal:
                    case HumanBodyBones.LeftMiddleProximal:
                    case HumanBodyBones.LeftRingProximal:
                    case HumanBodyBones.LeftLittleProximal:
                    case HumanBodyBones.RightIndexProximal:
                    case HumanBodyBones.RightMiddleProximal:
                    case HumanBodyBones.RightRingProximal:
                    case HumanBodyBones.RightLittleProximal:
                        SetLimits(bone.Joint, _jointLimitValues.FingersProximal);
                        break;
                    default:
                        SetLimits(bone.Joint, _jointLimitValues.Fingers);
                        break;
                }
            }
        }

        private void SetLimits(ConfigurableJoint joint, JointLimitData data) 
        {
            joint.angularXMotion = data.angularXMotion;
            joint.angularYMotion = data.angularYMotion;
            joint.angularZMotion = data.angularZMotion;

            SoftJointLimit xlowLimit = joint.lowAngularXLimit;
            SoftJointLimit xHighLimit = joint.highAngularXLimit;
            SoftJointLimit yLimit = joint.angularYLimit;
            SoftJointLimit zLimit = joint.angularZLimit;

            xlowLimit.limit = data.XMin;
            xHighLimit.limit = data.XMax;
            yLimit.limit = data.Y;
            zLimit.limit = data.Z;

            joint.lowAngularXLimit = xlowLimit;
            joint.highAngularXLimit = xHighLimit;
            joint.angularYLimit = yLimit;
            joint.angularZLimit = zLimit;
        }

        private void LooseForce() 
        {
            currentForceRatio -= _forceRatioGain * Time.fixedDeltaTime;
            currentForceRatio = Mathf.Clamp(currentForceRatio, 0, 1);
            _animFollow.ForceCoeficient = Mathf.Lerp(_minimumForceRatio, _maximumForceRatio, currentForceRatio);
        }

        private void GainForce() 
        {
            currentForceRatio += _forceRatioGain * Time.fixedDeltaTime;
            currentForceRatio = Mathf.Clamp(currentForceRatio, 0, 1);
            _animFollow.ForceCoeficient = Mathf.Lerp(_minimumForceRatio, _maximumForceRatio, currentForceRatio);
        }
    }
}
