using Dp.Rig.Interfaces;
using Dp.Hands;
using Dp.Extensions;
using UnityEngine;

namespace Dp.Rig.Bindings
{
    [System.Serializable]
    public class HandBinding : Binding
    {
        public Transform AvatarShoulder = null;
        public bool IsLeft = false;
        [Header("Elbow settings")]
        public Transform Hint;
        public Vector3 LowerPosition;
        public Vector3 UpperPosition;

        private float _armsLengthFromShoulder;
        private float _armDiff;

        public override void Initialze(IRigDataProvider rigDataProvider)
        {
            base.Initialze(rigDataProvider);
            _armDiff = -(RigDataProvider.ArmLenght.user - RigDataProvider.ArmLenght.target);

            _armsLengthFromShoulder = Vector3.Distance(AvatarShoulder.position, Target.position);

            IsInitialized = true;
        }

        public override void Bind()
        {
            // Adjust trackers position
            TrackerAdj.position = GetAjdustedPosition();
            UpdateTargetTransform();
            UpdateElbowPosition();
        }

        void UpdateTargetTransform() 
        {
            Target.position = GetConstrainedPositon();
            var visibleHand = IsLeft ? HandsManager.Instance?.IsHandVisible(true) : HandsManager.Instance?.IsHandVisible();
            
            if (visibleHand == false)
            {
                Target.rotation = Quaternion.RotateTowards(Target.rotation, GetRotation(), Time.deltaTime * 500f);
            }
        }

        Vector3 GetAjdustedPosition()
        {
            float distance = Vector3.Distance(RigDataProvider.Model.AvatarNeck.position , Tracker.position);
            
            float adjustLenght = distance.Remap(0, RigDataProvider.ArmLenght.user, 0, _armDiff);
            Vector3 dir = Tracker.position - RigDataProvider.Model.AvatarNeck.position;
            Vector3 clamp = Vector3.ClampMagnitude(dir, adjustLenght);
            return RigDataProvider.Model.CameraRig.TransformPoint(clamp);
        }

        Quaternion GetRotation() 
        {
            Quaternion correctHands =  Quaternion.Euler(RotationOffset);
            Quaternion ignoreSourceRotation = Source.rotation * Quaternion.Inverse(_sourceRotationOrigin);
            Quaternion targetRotation = Quaternion.Euler(_targetRotationOrigin.eulerAngles);
            return ignoreSourceRotation * correctHands * targetRotation;
        }

        Vector3 GetConstrainedPositon() 
        {
            Source.localPosition = PositionOffset;
            Vector3 position = Source.position;

            Vector3 shoulderPos = AvatarShoulder.position;
            float currentDistance = Vector3.Distance(shoulderPos, position);
            if (currentDistance > _armsLengthFromShoulder)
            {
                Vector3 v = position - shoulderPos;
                v = Vector3.ClampMagnitude(v, _armsLengthFromShoulder);
                return shoulderPos + v;
            } 
            else
                return position;
        }

        void UpdateElbowPosition() 
        {
            var dir = UpperPosition - LowerPosition;
            var t = Target.position.y - RigDataProvider.HeadsHeight.target;

            float y = t < LowerPosition.y ? LowerPosition.y : t > UpperPosition.y ? UpperPosition.y : t;
            float x = LowerPosition.x + (y * dir.x);
            float z = LowerPosition.z + (y * dir.z);

            Hint.localPosition = new Vector3(x, y, z);
        }      
    }
}
