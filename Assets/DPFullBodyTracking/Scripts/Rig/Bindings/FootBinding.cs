using Dp.Rig.Interfaces;
using UnityEngine;

namespace Dp.Rig.Bindings
{
    [System.Serializable]
    public class FootBinding : Binding
    {
        public bool IsLeft = false;
        public override void Initialze(IRigDataProvider rigDataProvider)
        {
            base.Initialze(rigDataProvider);
            float test = Target.position.y - Tracker.position.y;
            Source.rotation = Source.rotation * Quaternion.Inverse(_sourceRotationOrigin);
            TrackerAdj.localPosition = new Vector3(0f, test, 0f);
            Source.localPosition = (Source.localRotation * PositionOffset);
            IsInitialized = true;
        }

        public override void Bind()
        {
            var position = Source.position;
            var rotation = GetRotation();

            Target.SetPositionAndRotation(position, rotation);
        }

        Quaternion GetRotation() 
        {
            Quaternion deltaRot = Source.rotation;
            Quaternion rotOffset = Quaternion.Euler(RotationOffset + _targetRotationOrigin.eulerAngles);
            return deltaRot * rotOffset;
        }
    }
}
