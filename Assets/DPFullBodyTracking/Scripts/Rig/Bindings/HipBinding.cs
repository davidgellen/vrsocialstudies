using Dp.Rig.Interfaces;
using UnityEngine;
using Dp.Extensions;

namespace Dp.Rig.Bindings
{
    [System.Serializable]
    public class HipBinding : Binding
    {
        float heightAdj;

        public override void Initialze(IRigDataProvider rigDataProvider) 
        {
            base.Initialze(rigDataProvider);
            heightAdj = -(RigDataProvider.HipsHeight.user - RigDataProvider.HipsHeight.target);
            Source.rotation = Source.rotation * Quaternion.Inverse(_sourceRotationOrigin);

            IsInitialized = true;
        }

        public override void Bind()
        {
            TrackerAdj.position = AdjustPosition();
            var position = GetPosition();
            var rotation = GetRotation();

            Target.SetPositionAndRotation(position, rotation);
        }

        public Vector3 AdjustPosition() 
        {
            float adjustedHeight = Tracker.position.y.Remap(0, RigDataProvider.HipsHeight.target, 0, heightAdj);
            return new Vector3(TrackerAdj.position.x, adjustedHeight, TrackerAdj.position.z);
        }

        public Vector3 GetPosition()
        {
            Source.localPosition = Source.localRotation * PositionOffset;
            return Source.position;
        }

        public Quaternion GetRotation() 
        {
            Quaternion deltaRot = Source.rotation;
            Quaternion rotOffset = Quaternion.Euler(RotationOffset + _targetRotationOrigin.eulerAngles);
            return deltaRot * rotOffset;
        }
    }
}
