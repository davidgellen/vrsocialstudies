using UnityEngine;

namespace Dp.ActiveRagdoll.Data
{
    [System.Serializable]
    public class BoneSetup
    {
        [Header("Body parts")]
        public Transform Hips;
        public Transform Head;
        public Transform LeftHand;
        public Transform RightHand;
        public Transform LeftFoot;
        public Transform RightFoot;

        [Header("Left hand")]
        public Transform LeftThumb;
        public Transform LeftIndex;
        public Transform LeftMiddle;
        public Transform LeftRing;
        public Transform LeftPinky;

        [Header("Right hand")]
        public Transform RightThumb;
        public Transform RightIndex;
        public Transform RightMiddle;
        public Transform RightRing;
        public Transform RightPinky;

        [Header("Twist bones")]
        public Transform LeftUpperArmTwist;
        public Transform RightUpperArmTwist;
        public Transform LeftLowerArmTwist;
        public Transform RightLoverArmTwist;

        public bool BonesAssigned { get; set; }
    }
}
