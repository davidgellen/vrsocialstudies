using Dp.Rig.Interfaces;
using UnityEngine;

namespace Dp.Rig.Bindings
{
    [System.Serializable]
    public class HeadBinding : Binding
    {
        private Vector3 _trackerOffset;

        public override void Initialze(IRigDataProvider rigDataProvider)
        {
            base.Initialze(rigDataProvider);

            _trackerOffset = Target.position - Tracker.position;
            Source.rotation = Source.rotation * Quaternion.Inverse(_sourceRotationOrigin);

            IsInitialized = true;
        }

        public override void Bind()
        {
            var position = GetPosition();
            var rotation = GetRotation();

            Target.SetPositionAndRotation(position, rotation);
        }

        public Vector3 GetPosition() 
        {
            Source.localPosition = PositionOffset;
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
