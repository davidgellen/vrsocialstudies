using UnityEngine;

namespace Dp.ActiveRagdoll
{
    public class Bone
    {
        public Transform MasterBone;
        public Transform SlaveBone;
        public ConfigurableJoint Joint;
        public Rigidbody RB;
        public Vector3 CenterOfMassPosition;
        public (Quaternion startRotation, Quaternion direction) Orientation;

        public Bone(Transform slaveBone, Transform masterBone, bool root = false) 
        {
            SlaveBone = slaveBone;
            if(slaveBone.gameObject.TryGetComponent(out Collider collider))
                slaveBone.gameObject.AddComponent<CollisionHandler>();
            RB = slaveBone.GetComponent<Rigidbody>();
            MasterBone = masterBone;
            CenterOfMassPosition = GetCenterOfMassPosition();

            if (root)
                return;

            Joint = slaveBone.GetComponent<ConfigurableJoint>();
            Orientation = GetJointOrientation();
        }

        private Vector3 GetCenterOfMassPosition()
        {
            var diff = RB.worldCenterOfMass - SlaveBone.position;
            return Quaternion.Inverse(SlaveBone.rotation) * diff;
        }

        private (Quaternion startRotation, Quaternion direction) GetJointOrientation()
        {
            Vector3 forward = Vector3.Cross(Joint.axis, Joint.secondaryAxis);
            Vector3 up = Joint.secondaryAxis;

            var dir = Quaternion.LookRotation(forward, up);
            var rot = Joint.transform.localRotation;

            return (rot, dir);
        }
    }
}
