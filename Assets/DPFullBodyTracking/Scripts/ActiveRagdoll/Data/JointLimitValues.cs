using UnityEngine;

namespace Dp.ActiveRagdoll.Data
{
    [System.Serializable]
    public class JointLimitData 
    {
        public float XMin;
        public float XMax;
        public float Y;
        public float Z;
        public ConfigurableJointMotion angularXMotion = ConfigurableJointMotion.Limited;
        public ConfigurableJointMotion angularYMotion = ConfigurableJointMotion.Limited;
        public ConfigurableJointMotion angularZMotion = ConfigurableJointMotion.Limited;

        public JointLimitData(float XMin, float XMax, float Y, float Z , ConfigurableJointMotion angularXMotion, ConfigurableJointMotion angularYMotion, ConfigurableJointMotion angularZMotion) 
        {
            this.XMin = XMin;
            this.XMax = XMax;
            this.Y = Y;
            this.Z = Z;
            this.angularXMotion = angularXMotion;
            this.angularYMotion = angularYMotion;
            this.angularZMotion = angularZMotion;
        }
    }

    [System.Serializable]
    public class JointLimitValues
    {
        public JointLimitData UpperLeg = new JointLimitData(-90, 180, 90, 90, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited);
        public JointLimitData LowerLeg = new JointLimitData(-130, 0, 0, 0, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited);
        public JointLimitData Foot = new JointLimitData(-90, 20, 45, 45, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited);
        public JointLimitData Toes = new JointLimitData(0, 90, 0, 0, ConfigurableJointMotion.Locked, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Locked);

        public JointLimitData Shoulder = new JointLimitData(0, 0, 0, 0, ConfigurableJointMotion.Free, ConfigurableJointMotion.Free, ConfigurableJointMotion.Limited);
        public JointLimitData LowerArm = new JointLimitData(0, 0, 0, 150, ConfigurableJointMotion.Locked, ConfigurableJointMotion.Locked, ConfigurableJointMotion.Limited);

        public JointLimitData Fingers = new JointLimitData(0, 0, 0, 95, ConfigurableJointMotion.Locked, ConfigurableJointMotion.Locked, ConfigurableJointMotion.Limited);
        public JointLimitData FingersProximal = new JointLimitData(-180, 180, 45, 95, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited);
        public JointLimitData FingerThumb = new JointLimitData(-90, 90, 180, 0, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited, ConfigurableJointMotion.Locked);
        public JointLimitData FingerThumbProximal = new JointLimitData(-90, 90, 180, 0, ConfigurableJointMotion.Free, ConfigurableJointMotion.Free, ConfigurableJointMotion.Free);
    }
}