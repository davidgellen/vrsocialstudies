using UnityEngine;
using ViveHandTracking;

namespace Dp.Rig.Interfaces
{
    public interface IRigDataProvider
    {
        public IRigModel Model { get; }
        public (Vector3 user, Vector3 target) NeckPosition { get; }
        public (float user, float target) HipsHeight { get; }
        public (float user, float target) HeadsHeight { get; }
        public float RigScale { get; }
        public float ArmScale { get; }
        public (float left, float right) FootScale { get; }
        public (float user, float target) ArmLenght { get; }
    }
}
