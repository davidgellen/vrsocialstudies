using UnityEngine;
using UnityEditor;
using UnityEngine.Animations.Rigging;
using System.Linq;
using UnityEngine.Animations;

namespace DPEditor
{
    public partial class VRRigEditor 
    {
        private static readonly string RIG_NAME = "VRRig";

        private bool _bonesMapperStarted = false;
        private Transform _headController;
        private Transform _hipsController;
        private Transform _leftArmController;
        private Transform _rightArmController;
        private Transform _leftLegController;
        private Transform _rightLegController;

        public void ConstraintBuilderInspectorGUI()
        {
            if (_target.gameObject.transform.Find(RIG_NAME))
                return;

            //draw bones data
            EditorGUILayout.BeginHorizontal(GUI.skin.box); EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(m_BonesData, true);
            }
            EditorGUI.indentLevel--; EditorGUILayout.EndHorizontal();

            DrawButtons();

            m_Target.ApplyModifiedProperties();
            m_Target.UpdateIfRequiredOrScript();
        }

        private void DrawButtons() 
        {
            // Draw error
            if (_bonesMapperStarted && !_target.BonesData.IsValid)
                EditorGUILayout.HelpBox("Some bones were not found or are missing! Please assign it manually.", MessageType.Error, true);
            else if (_target.BonesData.IsValid)
                EditorGUILayout.HelpBox("Please check that the bones are correctly assigned.", MessageType.Warning, true);

            //draw button
            if (!_target.BonesData.IsValid)
            {
                if (GUILayout.Button("Assign bones")) MapBonesData();
            }
            else
            {
                if (GUILayout.Button("Build constraints")) BuildConstraints();
            }
        }

        private void MapBonesData() 
        {
            _bonesMapperStarted = true;
            var childs = _target.GetComponentsInChildren<Transform>();
            _target.BonesData.Hips = childs.Where(child => child.name.ToLower().Contains(BoneNames.HIPS)).FirstOrDefault();
            _target.BonesData.Spine = childs.Where(child => child.name.ToLower().Contains(BoneNames.SPINE)).FirstOrDefault();
            _target.BonesData.Head = childs.Where(child => child.name.ToLower().Contains(BoneNames.HEAD)).FirstOrDefault();
            _target.BonesData.HeadTop = childs.Where(child => child.name.ToLower().Contains(BoneNames.HEAD_TOP)).FirstOrDefault();
            _target.BonesData.LeftArm = childs.Where(child => child.name.ToLower().Contains(BoneNames.LEFT_ARM)).FirstOrDefault();
            _target.BonesData.RightArm = childs.Where(child => child.name.ToLower().Contains(BoneNames.RIGHT_ARM)).FirstOrDefault();
            _target.BonesData.LeftUpLeg = childs.Where(child => child.name.ToLower().Contains(BoneNames.LEFT_UPLEG)).FirstOrDefault();
            _target.BonesData.RightUpLeg = childs.Where(child => child.name.ToLower().Contains(BoneNames.RIGHT_UPLEG)).FirstOrDefault();

            _target.BonesData.TwistLeftLowerArm = _target.BonesData.LeftArm.GetComponentsInChildren<Transform>().Where(child => child.name.ToLower().Contains(BoneNames.TWIST_ARM)).FirstOrDefault();
            _target.BonesData.TwistLeftUpperArm = _target.BonesData.LeftArm.GetComponentsInChildren<Transform>().Where(child => child.name.ToLower().Contains(BoneNames.TWIST_ARM)).FirstOrDefault();
            _target.BonesData.TwistRightLowerArm = _target.BonesData.RightArm.GetComponentsInChildren<Transform>().Where(child => child.name.ToLower().Contains(BoneNames.TWIST_FOREARM)).FirstOrDefault();
            _target.BonesData.TwistRightUpperArm = _target.BonesData.RightArm.GetComponentsInChildren<Transform>().Where(child => child.name.ToLower().Contains(BoneNames.TWIST_FOREARM)).FirstOrDefault();

            m_Target.Update();
            Repaint();
        }

        private void BuildConstraints() 
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("build constraints");
            var undoGroupIndex = Undo.GetCurrentGroup();

            AddAnimationRiggingComponents();
            AddConstraints();
            AlignControllers();
            AssignControllers();
            AssingHandData();

            _rigExists = _target.gameObject.transform.Find(RIG_NAME);
            Undo.CollapseUndoOperations(undoGroupIndex);
        }

        private void AddAnimationRiggingComponents() 
        {            
            Rig rig;
            RigBuilder rigBuilder;

            Transform _vrRigObject = _target.gameObject.transform.Find(RIG_NAME);
            if (_vrRigObject == null)
                _vrRigObject = EditorUtils.NewGameObjectWithParent(RIG_NAME, _target.transform).transform;

            if (!_vrRigObject.TryGetComponent(out rig))
                rig = Undo.AddComponent<Rig>(_vrRigObject.gameObject);

            if (!_target.TryGetComponent(out rigBuilder))
            {
                rigBuilder = Undo.AddComponent<RigBuilder>(_target.gameObject);
                if (rigBuilder.layers.Count == 0)
                {
                    rigBuilder.layers.Add(new RigLayer(rig, true));
                }
            }
        }

        private void AddConstraints() 
        {
            HeadConstraint();
            RootConstraint();
            ArmConstraint();
            ArmConstraint(true);
            LegConstraint();
            LegConstraint(true);
        }

        private void HeadConstraint() 
        {
            string CONSTRAINT_NAME = "HeadConstraint";
            string CONTROLLER_NAME = "HeadController";

            Transform _vrRigObject = _target.gameObject.transform.Find(RIG_NAME);
            var constraint = EditorUtils.NewGameObjectWithParent(CONSTRAINT_NAME, _vrRigObject);
            _headController = EditorUtils.NewGameObjectWithParent(CONTROLLER_NAME, constraint.transform).transform;

            // Add MultiParentConstraint
            MultiParentConstraint multiParentConstraint = Undo.AddComponent<MultiParentConstraint>(constraint);
            multiParentConstraint.data.constrainedObject = _target.BonesData.HeadTop;
            var sourceObjects = multiParentConstraint.data.sourceObjects;
            sourceObjects.Clear();
            sourceObjects.Add(new WeightedTransform(_headController, 1));
            multiParentConstraint.data.sourceObjects = sourceObjects;

            // Add MultiRotationConstraint
            MultiRotationConstraint multiRotationConstraint = Undo.AddComponent<MultiRotationConstraint>(constraint);
            multiRotationConstraint.data.constrainedObject = _target.BonesData.HeadTop;
            sourceObjects = multiRotationConstraint.data.sourceObjects;
            sourceObjects.Clear();
            sourceObjects.Add(new WeightedTransform(_target.BonesData.Head, 1));
            multiRotationConstraint.data.sourceObjects = sourceObjects;
        }

        private void RootConstraint() 
        {
            string CONSTRAINT_NAME = "Root";
            string CONTROLLER_NAME = "HipsController";

            Transform _vrRigObject = _target.gameObject.transform.Find(RIG_NAME);
            var constraint = EditorUtils.NewGameObjectWithParent(CONSTRAINT_NAME, _vrRigObject);
            _hipsController = EditorUtils.NewGameObjectWithParent(CONTROLLER_NAME, constraint.transform).transform;

            // Add MultiParentConstraint
            MultiParentConstraint multiParentConstraint = Undo.AddComponent<MultiParentConstraint>(constraint);
            multiParentConstraint.data.constrainedObject = _target.BonesData.Hips;
            var sourceObjects = multiParentConstraint.data.sourceObjects;
            sourceObjects.Clear();
            sourceObjects.Add(new WeightedTransform(_hipsController.transform, 1));
            multiParentConstraint.data.sourceObjects = sourceObjects;

            // Add ChainIKConstraint
            ChainIKConstraint chainIKConstraint = Undo.AddComponent<ChainIKConstraint>(constraint);
            chainIKConstraint.data.root = _target.BonesData.Spine;
            chainIKConstraint.data.tip = _target.BonesData.Head;
            chainIKConstraint.data.target = _headController;
            chainIKConstraint.data.chainRotationWeight = 0.7f;
        }

        private void ArmConstraint(bool isLeft = false) 
        {
            string CONSTRAINT_NAME = isLeft ? "LeftArmIK" : "RightArmIK";
            string CONTROLLER_NAME = isLeft ? "LeftArmController" : "RightArmController";
            string HINT_NAME = isLeft ? "LeftArmHint" : "RightArmHint";

            Transform _vrRigObject = _target.gameObject.transform.Find(RIG_NAME);
            var constraint = EditorUtils.NewGameObjectWithParent(CONSTRAINT_NAME, _vrRigObject);      
            if (isLeft)
                _leftArmController = EditorUtils.NewGameObjectWithParent(CONTROLLER_NAME, constraint.transform).transform;
            else
                _rightArmController = EditorUtils.NewGameObjectWithParent(CONTROLLER_NAME, constraint.transform).transform;

            var controller = isLeft ? _leftArmController : _rightArmController;
            var hint = EditorUtils.NewGameObjectWithParent(HINT_NAME, _hipsController).transform;

            //chainIk
            ShoulderConstraint chainIKConstraint = Undo.AddComponent<ShoulderConstraint>(constraint);
            chainIKConstraint.data.root = isLeft ? _target.BonesData.LeftArm.parent : _target.BonesData.RightArm.parent;
            chainIKConstraint.data.tip = isLeft ? _target.BonesData.LeftArm.GetChild(0) : _target.BonesData.RightArm.GetChild(0);
            chainIKConstraint.data.target = controller;
            chainIKConstraint.data.tipRotationWeight = 0;
            chainIKConstraint.data.chainRotationWeight = 0.5f;

            //Add TwistCorrection
            if (_target.BonesData.TwistRightLowerArm != null || _target.BonesData.TwistLeftLowerArm != null) {
                TwistCorrection twistCorrection = Undo.AddComponent<TwistCorrection>(constraint);
                twistCorrection.data.sourceObject = controller;
                var twistNodes = twistCorrection.data.twistNodes;
                twistNodes.Clear();
                twistNodes.Add(new WeightedTransform(isLeft ? _target.BonesData.TwistLeftLowerArm : _target.BonesData.TwistRightLowerArm, 0.58f));
                twistNodes.Add(new WeightedTransform(isLeft ? _target.BonesData.LeftArm.GetChild(0) : _target.BonesData.RightArm.GetChild(0), 0.63f));
                twistNodes.Add(new WeightedTransform(isLeft ? _target.BonesData.TwistLeftUpperArm : _target.BonesData.TwistRightUpperArm, 0.25f));
                twistNodes.Add(new WeightedTransform(isLeft ? _target.BonesData.LeftArm : _target.BonesData.RightArm, 0.10f));

                twistCorrection.data.twistNodes = twistNodes;
                twistCorrection.data.twistAxis = TwistCorrectionData.Axis.X;
            }
            // Add Two Bone IK
            TwoBoneIKConstraint twoBoneIKConstraint = Undo.AddComponent<TwoBoneIKConstraint>(constraint);
            twoBoneIKConstraint.data.target = controller;
            twoBoneIKConstraint.data.hint = hint;

            var tip = isLeft ? _target.BonesData.LeftArm.GetChild(0).GetChild(0) : _target.BonesData.RightArm.GetChild(0).GetChild(0);
            twoBoneIKConstraint.data.tip = tip;
            twoBoneIKConstraint.data.mid = tip.parent;
            twoBoneIKConstraint.data.root = tip.parent.parent;

            if (isLeft) 
            {
                _target.Model.LeftArm.AvatarShoulder = _target.BonesData.LeftArm.parent;
                _target.Model.LeftArm.Hint = hint;
                _target.Model.LeftArm.IsLeft = true;
            } 
            else 
            {
                _target.Model.RightArm.AvatarShoulder = _target.BonesData.RightArm.parent;
                _target.Model.RightArm.Hint = hint;
            }        
        }

        private void LegConstraint(bool isLeft = false)
        {
            string CONSTRAINT_NAME = isLeft ? "LeftLegIK" : "RightLegIK";
            string CONTROLLER_NAME = isLeft ? "LeftLegController" : "RightLegController";
            string HINT_NAME = isLeft ? "LeftLegHint" : "RightLegHint";

            Transform _vrRigObject = _target.gameObject.transform.Find(RIG_NAME);
            var constraint = EditorUtils.NewGameObjectWithParent(CONSTRAINT_NAME, _vrRigObject);
            if (isLeft)
                _leftLegController = EditorUtils.NewGameObjectWithParent(CONTROLLER_NAME, constraint.transform).transform;
            else
                _rightLegController = EditorUtils.NewGameObjectWithParent(CONTROLLER_NAME, constraint.transform).transform;

            var controller = isLeft ? _leftLegController : _rightLegController;
            var hint = EditorUtils.NewGameObjectWithParent(HINT_NAME, controller).transform;
            AlignTransform(hint, isLeft ? _target.BonesData.LeftUpLeg.GetChild(0) : _target.BonesData.RightUpLeg.GetChild(0));
            hint.transform.position = hint.forward * 2f;
            // Add Two Bone IK
            TwoBoneIKConstraint twoBoneIKConstraint = Undo.AddComponent<TwoBoneIKConstraint>(constraint);
            twoBoneIKConstraint.data.target = controller;
            twoBoneIKConstraint.data.hint = hint;

            var tip = isLeft ? _target.BonesData.LeftUpLeg.GetChild(0).GetChild(0) : _target.BonesData.RightUpLeg.GetChild(0).GetChild(0);
            twoBoneIKConstraint.data.tip = tip;
            twoBoneIKConstraint.data.mid = tip.parent;
            twoBoneIKConstraint.data.root = tip.parent.parent;

            if (isLeft) 
                _target.Model.LeftLeg.IsLeft = true; 
            else
                _target.Model.RightLeg.IsLeft = false;
        }

        private void AlignControllers() 
        {
            AlignTransform(_headController, _target.BonesData.Head);
            AlignTransform(_hipsController, _target.BonesData.Hips);
            AlignTransform(_leftArmController, _target.BonesData.LeftArm.GetChild(0).GetChild(0));
            AlignTransform(_rightArmController, _target.BonesData.RightArm.GetChild(0).GetChild(0));
            AlignTransform(_leftLegController, _target.BonesData.LeftUpLeg.GetChild(0).GetChild(0));
            AlignTransform(_rightLegController, _target.BonesData.RightUpLeg.GetChild(0).GetChild(0));
        }

        private void AssignControllers() 
        {
            _target.Model.Head.Target = _headController;
            _target.Model.Hips.Target = _hipsController;
            _target.Model.LeftArm.Target = _leftArmController;
            _target.Model.RightArm.Target = _rightArmController;
            _target.Model.LeftLeg.Target = _leftLegController;
            _target.Model.RightLeg.Target = _rightLegController;
        }

        private void AssingHandData() 
        {
            _target.Model.LeftHand.Palm = _target.BonesData.LeftArm.GetChild(0).GetChild(0);
            _target.Model.LeftHand.Thumb = _target.Model.LeftHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("thumb")).FirstOrDefault();
            _target.Model.LeftHand.Index = _target.Model.LeftHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("index")).FirstOrDefault();
            _target.Model.LeftHand.Middle = _target.Model.LeftHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("middle")).FirstOrDefault();
            _target.Model.LeftHand.Ring = _target.Model.LeftHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("ring")).FirstOrDefault();
            _target.Model.LeftHand.Pinky = _target.Model.LeftHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("pinky")).FirstOrDefault();

            _target.Model.RightHand.Palm = _target.BonesData.RightArm.GetChild(0).GetChild(0);
            _target.Model.RightHand.Thumb = _target.Model.RightHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("thumb")).FirstOrDefault();
            _target.Model.RightHand.Index = _target.Model.RightHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("index")).FirstOrDefault();
            _target.Model.RightHand.Middle = _target.Model.RightHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("middle")).FirstOrDefault();
            _target.Model.RightHand.Ring = _target.Model.RightHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("ring")).FirstOrDefault();
            _target.Model.RightHand.Pinky = _target.Model.RightHand.Palm.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains("pinky")).FirstOrDefault();

        }

        private void AlignTransform(Transform source, Transform destination)
        {
            Undo.RecordObject(source, "");
            source.SetPositionAndRotation(destination.position, destination.rotation);
        }

    }
}