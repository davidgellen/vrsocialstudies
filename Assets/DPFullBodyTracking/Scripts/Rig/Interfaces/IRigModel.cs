using Dp.Rig.Bindings;
using UnityEngine;
using ViveHandTracking;

namespace Dp.Rig.Interfaces
{
    public interface IRigModel
    {
        public Transform CameraRig { get; }
        public Transform AvatarNeck { get; }
        public float AvatarHeight { get; }

        // controllers
        public HeadBinding Head { get; }
        public HipBinding Hips { get; }
        public HandBinding LeftArm { get; }
        public HandBinding RightArm { get; }
        public FootBinding LeftLeg { get; }
        public FootBinding RightLeg { get; }

        // providers
        public GestureProvider GetGestureProvider { get; }
              
        public void Initialize();
    }
}
