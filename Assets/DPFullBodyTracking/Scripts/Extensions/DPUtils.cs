using Dp.Enums;
using UnityEngine;

namespace Dp.Extensions
{
    public static class DPUtils
    {

        public static void FreezeRotationOnAxis(this Rigidbody rb, Axis axis) 
        {
            switch (axis) 
            {
                case Axis.X:
                    rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    break;
                case Axis.Y:
                    rb.constraints = RigidbodyConstraints.FreezeRotationY;
                    break;
                case Axis.Z:
                    rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                    break;
                case Axis.None:
                    rb.constraints = RigidbodyConstraints.None;
                    break;
                default:
                    Debug.LogError("Invalid axis!");
                    break;
            }
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}
