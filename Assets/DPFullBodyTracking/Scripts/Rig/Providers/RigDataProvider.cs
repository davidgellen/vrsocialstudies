using Dp.Rig.Interfaces;
using System;
using UnityEngine;

namespace Dp.Rig.Providers
{
    public class RigDataProvider : IRigDataProvider
    {
        public IRigModel Model { get; private set; }
        public (float user, float target) HipsHeight { get; private set; }
        public (float user, float target) HeadsHeight { get; private set; }
        public float RigScale { get; private set; }
        public float ArmScale { get; private set; }
        public (float user, float target) ArmLenght { get; private set; }
        public (float left, float right) FootScale { get; private set; }
        public (Vector3 user, Vector3 target) NeckPosition => GetNeckPosition();
        
        public RigDataProvider(IRigModel model) 
        {
            Model = model;
          
            HipsHeight = (user: Model.Hips.Source.parent.position.y, target: Model.Hips.Target.position.y);
            HeadsHeight = (user: Camera.main.transform.localPosition.y, target: Model.AvatarHeight);
            RigScale = Camera.main.transform.parent.localScale.x;

            ArmScale = GetArmScale(
                NeckPosition, 
                (Model.LeftArm.Source.parent.position, Model.LeftArm.Target.position));
            FootScale = (GetFootScale(true), GetFootScale(false));
        }

        /// <summary>
        /// Get neck position
        /// </summary>
        /// <returns>Tuple<Vector3, Vector3>. 1st item is user neck position and 2nd is avatar neck position</returns>
        public (Vector3 user, Vector3 target) GetNeckPosition() 
        {
            // user neck
            var userNeckPosition = new Vector3(
                Model.Head.Source.parent.localPosition.x, 
                Model.Head.Source.parent.localPosition.y - 0.3f, 
                Model.Head.Source.parent.localPosition.z);

            // avatar neck
            var avatarNeckPosition = Model.CameraRig.InverseTransformPoint(Model.AvatarNeck.position);


            return (userNeckPosition, avatarNeckPosition);
        }

        /// <summary>
        /// Get scale between two distances
        /// </summary>
        /// <param name="point1">ValueTuple(Vector3, Vector3).. two points for distance</param>
        /// <param name="point2">ValueTuple(Vector3, Vector3).. two points for distance</param>
        /// <returns>float</returns>
        public float GetArmScale((Vector3, Vector3) point1, (Vector3, Vector3) point2) 
        {
            float source = Vector3.Distance(point1.Item1, point2.Item1);
            float target = Vector3.Distance(point1.Item2, point2.Item2);
            ArmLenght = (source, target);
            return target / source;
        }

        /// <summary>
        /// Get foot scale
        /// </summary>
        /// <returns>float</returns>
        public float GetFootScale(bool isLeft) 
        {
            Transform leg = isLeft ? Model.LeftLeg.Source.parent : Model.RightLeg.Source.parent;
            Transform avatarleg = isLeft ? Model.LeftLeg.Target : Model.RightLeg.Target;

            var userFoot = new Vector3(
                Model.Hips.Source.parent.position.x,
                leg.position.y, 
                Model.Hips.Source.parent.position.z);

            var avatarFoot = new Vector3(
                Model.Hips.Target.position.x,
                avatarleg.position.y, 
                Model.Hips.Target.position.z);

            float source = Vector3.Distance(Model.Hips.Source.parent.position, userFoot);
            float target = Vector3.Distance(Model.Hips.Target.position, avatarFoot);
            return target / source;
        }
    }
}
