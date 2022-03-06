using Dp.Rig.Interfaces;
using UnityEngine;

namespace Dp.Rig.Bindings
{
    public abstract class Binding
    {
        [HideInInspector] public Transform Tracker;
        [HideInInspector] public Transform TrackerAdj;
        public Transform Source = null;
        public Transform Target = null;
        public Vector3 PositionOffset = Vector3.zero;
        public Vector3 RotationOffset = Vector3.zero;

        protected Quaternion _sourceRotationOrigin;
        protected Quaternion _targetRotationOrigin;

        public bool IsInitialized { get; set; } = false;
        public IRigDataProvider RigDataProvider { get; set; }

        public bool IsValid() => Source != null && Target != null;
        public bool TryBind()
        {
            if (!IsInitialized || Source == null || Target == null) 
            {
                Debug.LogWarning("Binder is not initialized.");
                return false;
            }

            bool isValid = IsValid();
            if (isValid)
                Bind();
            return isValid;
        }

        public virtual void Initialze(IRigDataProvider rigDataProvider) 
        {
            RigDataProvider = rigDataProvider;
            Tracker = Source.parent;
            TrackerAdj = Tracker.parent;
            _sourceRotationOrigin = Source.rotation;
            _targetRotationOrigin = Target.rotation;
        }

        public abstract void Bind();
    }
}
