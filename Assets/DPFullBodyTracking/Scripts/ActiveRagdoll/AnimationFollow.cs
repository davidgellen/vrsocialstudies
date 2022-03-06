using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using Dp.ActiveRagdoll.Data;

namespace Dp.ActiveRagdoll
{
    [Serializable]
    public class AnimationFollow
    {
        private ActiveRagdoll _controller;
        private Vector3[] _lastError;

        [SerializeField, Range(0f, 100f), Header("Animation setting")] private float _force = 10f;
        [SerializeField] private float RootPValue = 25f;
        [SerializeField] private float DriveSpring = 100f;
        [SerializeField] private float DriveDamper = 10f;

        [SerializeField, Range(0f, 100f)] private float[] _pValue;
        [SerializeField, Range(0f, 1f)] private float[] _dValue;

        public float ForceCoeficient { get; set; } = 1f;

        public void InitializeModel(ActiveRagdoll controller) 
        {
            _controller = controller;
            int length = Enum.GetNames(typeof(HumanBodyBones)).Length;
            _lastError = new Vector3[length];
            _pValue = Enumerable.Repeat(10f, length).ToArray();
            _dValue = Enumerable.Repeat(0.025f, length).ToArray();
            SetPIDParameters();
        }

        public void FollowAnimation() 
        {
            SetJointDrive();

            foreach (KeyValuePair<HumanBodyBones, Bone> pair in _controller.BonesDict)
            {
                var i = (int)pair.Key;
                var bone = pair.Value;

                Vector3 masterCenterOfMass = bone.MasterBone.position + bone.MasterBone.rotation * bone.CenterOfMassPosition;
                Vector3 error = masterCenterOfMass - bone.RB.worldCenterOfMass;
                Vector3 output = PIDControl(_pValue[i], _dValue[i], error, ref _lastError[i]);
                output = Vector3.ClampMagnitude(output, _force * ForceCoeficient);

                bone.RB.AddForce(output, ForceMode.VelocityChange);

                if (i != 0)
                {
                    var jointOrientation = bone.Orientation;
                    bone.Joint.targetRotation =
                        Quaternion.Inverse(jointOrientation.direction) *
                        Quaternion.Inverse(bone.MasterBone.localRotation) *
                        jointOrientation.startRotation;

                }
            }
        }

        private void SetJointDrive()
        {
            JointDrive drive;

            foreach (var item in _controller.BonesDict.Values.Skip(1))
            {
                item.Joint.rotationDriveMode = RotationDriveMode.XYAndZ;
                drive = item.Joint.angularXDrive;

                drive.positionSpring = DriveSpring;
                drive.positionDamper = DriveDamper;
                item.Joint.angularXDrive = drive;

                drive = item.Joint.angularYZDrive;

                drive.positionSpring = DriveSpring;
                drive.positionDamper = DriveDamper;
                item.Joint.angularYZDrive = drive;
            }
        }

        private Vector3 PIDControl(float P, float D, Vector3 error, ref Vector3 lastError)
        {
            Vector3 signal = P * (error + D * (error - lastError) / Time.fixedDeltaTime);
            lastError = error;
            return signal;
        }

        private void SetPIDParameters() 
        {
            int multiplier = 0;
            foreach (var key in BonesDictionary.RootBones)
            {
                multiplier++;
                int i = (int)key;
                float value = 0f;
                switch (key)
                {
                    case HumanBodyBones.Hips:
                        value = RootPValue + _controller.BonesDict[HumanBodyBones.Hips].RB.mass * multiplier;
                        _pValue[i] = value;
                        break;
                    case HumanBodyBones.Spine:
                        value = _pValue[(int)HumanBodyBones.Hips] + _controller.BonesDict[HumanBodyBones.Spine].RB.mass * multiplier;
                        _pValue[i] = value;
                        break;
                    case HumanBodyBones.Chest:
                        value = _pValue[(int)HumanBodyBones.Spine] + _controller.BonesDict[HumanBodyBones.Chest].RB.mass * multiplier;
                        _pValue[i] = value;
                        break;
                    case HumanBodyBones.UpperChest:
                        value = _pValue[(int)HumanBodyBones.Chest] + _controller.BonesDict[HumanBodyBones.UpperChest].RB.mass * multiplier;
                        _pValue[i] = value;
                        break;
                    case HumanBodyBones.Neck:
                        value = _pValue[(int)HumanBodyBones.UpperChest] + _controller.BonesDict[HumanBodyBones.Neck].RB.mass * multiplier;
                        _pValue[i] = value;
                        break;
                    case HumanBodyBones.Head:
                        value = _pValue[(int)HumanBodyBones.Neck] + _controller.BonesDict[HumanBodyBones.Head].RB.mass * multiplier;
                        _pValue[i] = value;
                        break;
                }
            }

            multiplier = 0;
            foreach (var key in BonesDictionary.RightArmBones)
            {
                multiplier++;
                int i = (int)key;
                float value = 0f;
                switch (key)
                {
                    case HumanBodyBones.RightShoulder:
                        value = _pValue[(int)HumanBodyBones.UpperChest] + _controller.BonesDict[HumanBodyBones.RightShoulder].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftShoulder] = value;
                        break;
                    case HumanBodyBones.RightUpperArm:
                        value = _pValue[(int)HumanBodyBones.RightShoulder] + _controller.BonesDict[HumanBodyBones.RightUpperArm].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftUpperArm] = value;
                        break;
                    case HumanBodyBones.RightLowerArm:
                        value = _pValue[(int)HumanBodyBones.RightUpperArm] + _controller.BonesDict[HumanBodyBones.RightLowerArm].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftLowerArm] = value;
                        break;
                    case HumanBodyBones.RightHand:
                        value = _pValue[(int)HumanBodyBones.RightLowerArm] + _controller.BonesDict[HumanBodyBones.RightHand].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftHand] = value;
                        break;
                }
            }

            multiplier = 0;
            foreach (var key in BonesDictionary.RightLegBones)
            {
                multiplier++;
                int i = (int)key;
                float value = 0f;
                switch (key)
                {
                    case HumanBodyBones.RightUpperLeg:
                        value = _pValue[(int)HumanBodyBones.Hips] + _controller.BonesDict[HumanBodyBones.RightUpperLeg].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftUpperLeg] = value;
                        break;
                    case HumanBodyBones.RightLowerLeg:
                        value = _pValue[(int)HumanBodyBones.RightUpperLeg] + _controller.BonesDict[HumanBodyBones.RightLowerLeg].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftLowerLeg] = value;
                        break;
                    case HumanBodyBones.RightFoot:
                        value = _pValue[(int)HumanBodyBones.RightLowerLeg] + _controller.BonesDict[HumanBodyBones.RightFoot].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftFoot] = value;
                        break;
                    case HumanBodyBones.RightToes:
                        value = _pValue[(int)HumanBodyBones.RightFoot] + _controller.BonesDict[HumanBodyBones.RightToes].RB.mass * multiplier;
                        _pValue[i] = _pValue[(int)HumanBodyBones.LeftToes] = value;
                        break;
                }
            }

            var fingers = BonesDictionary.LeftHandBones.Concat(BonesDictionary.RightHandBones);
            foreach (var item in fingers)
            {
                _pValue[(int)item] = _pValue[(int)HumanBodyBones.LeftHand];
            }
        }
    }
}
