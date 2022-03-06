using UnityEngine;

namespace Dp.Rig.Model
{
    [System.Serializable]
    public class ConstrainedBonesData
    {
        public Transform Hips;
        public Transform Spine;
        public Transform Head;
        public Transform HeadTop;
        public Transform LeftArm;
        public Transform RightArm;
        public Transform LeftUpLeg;
        public Transform RightUpLeg;

        public Transform TwistLeftUpperArm;
        public Transform TwistRightUpperArm;
        public Transform TwistLeftLowerArm;
        public Transform TwistRightLowerArm;

        public bool IsValid =>
            Hips != null &&
            Spine != null &&
            Head != null &&
            HeadTop != null &&
            LeftArm != null &&
            RightArm != null &&
            LeftUpLeg != null &&
            RightUpLeg != null;
    }
}
