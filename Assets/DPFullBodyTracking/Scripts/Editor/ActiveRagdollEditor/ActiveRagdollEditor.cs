using Dp.ActiveRagdoll;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

namespace DPEditor
{
    [CustomEditor(typeof(ActiveRagdoll))]
    public class ActiveRagdollEditor : Editor
    {
        private ActiveRagdoll _target;

        SerializedProperty m_slaveBones;
        SerializedProperty m_masterRoot;
        SerializedProperty m_jointDirection;
        SerializedProperty m_enableLimits;
        SerializedProperty m_jointLimitValues;

        SerializedProperty m_IgnoreCollisionMask;
        SerializedProperty m_forceRatioGain;
        SerializedProperty m_minimumForceRatio;
        SerializedProperty m_maximumForceRatio;

        SerializedProperty m_force;
        SerializedProperty m_pStartValue;
        SerializedProperty m_driveSpring;
        SerializedProperty m_driveDamper;

        private void OnEnable()
        {
            _target = (ActiveRagdoll)target;
            m_slaveBones = serializedObject.FindProperty("SlaveBones");
            m_masterRoot = serializedObject.FindProperty("MasterRoot");
            m_jointDirection = serializedObject.FindProperty("JointDirection");
            m_enableLimits = serializedObject.FindProperty("EnableLimits");
            m_jointLimitValues = serializedObject.FindProperty("_jointLimitValues");

            m_IgnoreCollisionMask = serializedObject.FindProperty("_CollisionMask");
            m_forceRatioGain = serializedObject.FindProperty("_forceRatioGain");
            m_minimumForceRatio = serializedObject.FindProperty("_minimumForceRatio");
            m_maximumForceRatio = serializedObject.FindProperty("_maximumForceRatio");

            m_force = serializedObject.FindProperty("_animFollow").FindPropertyRelative("_force");
            m_pStartValue = serializedObject.FindProperty("_animFollow").FindPropertyRelative("RootPValue");
            m_driveSpring = serializedObject.FindProperty("_animFollow").FindPropertyRelative("DriveSpring");
            m_driveDamper = serializedObject.FindProperty("_animFollow").FindPropertyRelative("DriveDamper");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("Please check that the bones are correctly assigned.", MessageType.Warning, true);
            EditorGUILayout.BeginVertical(GUI.skin.box); EditorGUI.indentLevel++;
            {               
                EditorGUILayout.PropertyField(m_slaveBones, true);
                if (GUILayout.Button("Assign bones")) MapBonesData();
                if (_target.SlaveBones.BonesAssigned)
                {
                    if (GUILayout.Button("Generate ragdoll"))
                    {
                        Undo.IncrementCurrentGroup();
                        Undo.SetCurrentGroupName("Generate ragdoll");
                        var undoGroupIndex = Undo.GetCurrentGroup();
                        GenerateColliders();
                        GenerateJoints();
                        AssignLayersAndTags();
                        AddTwistCorrection();
                        Undo.CollapseUndoOperations(undoGroupIndex);

                        m_slaveBones.isExpanded = false;
                    }
                }
            }
            EditorGUI.indentLevel--; EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box); EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(m_masterRoot, new GUIContent("Master root (Hips)"), true);
                EditorGUILayout.PropertyField(m_jointDirection);
                EditorGUILayout.PropertyField(m_enableLimits);
                if (_target.EnableLimits) 
                {
                    EditorGUILayout.PropertyField(m_jointLimitValues, true);
                }
            }
            EditorGUI.indentLevel--; EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box); EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(m_IgnoreCollisionMask);
                EditorGUILayout.PropertyField(m_forceRatioGain);
                EditorGUILayout.PropertyField(m_minimumForceRatio);
                EditorGUILayout.PropertyField(m_maximumForceRatio);
            }
            EditorGUI.indentLevel--; EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box); EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(m_force);
                EditorGUILayout.PropertyField(m_pStartValue);
                EditorGUILayout.PropertyField(m_driveSpring);
                EditorGUILayout.PropertyField(m_driveDamper);
            }
            EditorGUI.indentLevel--; EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        private void MapBonesData()
        {
            _target.SlaveBones.BonesAssigned = true;
            var childs = _target.GetComponentsInChildren<Transform>();
            _target.SlaveBones.Hips = childs.Where(child => child.name.ToLower().Contains(BoneNames.HIPS)).FirstOrDefault();
            _target.SlaveBones.Head = childs.Where(child => child.name.ToLower().Contains(BoneNames.HEAD)).FirstOrDefault();
            _target.SlaveBones.LeftHand = childs.Where(child => child.name.ToLower().Contains(BoneNames.LEFT_HAND)).FirstOrDefault();
            _target.SlaveBones.RightHand = childs.Where(child => child.name.ToLower().Contains(BoneNames.RIGHT_HAND)).FirstOrDefault();
            _target.SlaveBones.LeftFoot = childs.Where(child => child.name.ToLower().Contains(BoneNames.LEFT_FOOT)).FirstOrDefault();
            _target.SlaveBones.RightFoot = childs.Where(child => child.name.ToLower().Contains(BoneNames.RIGHT_FOOT)).FirstOrDefault();

            _target.SlaveBones.LeftThumb = _target.SlaveBones.LeftHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.THUMB)).FirstOrDefault();
            _target.SlaveBones.LeftIndex = _target.SlaveBones.LeftHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.INDEX)).FirstOrDefault();
            _target.SlaveBones.LeftMiddle = _target.SlaveBones.LeftHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.MIDDLE)).FirstOrDefault();
            _target.SlaveBones.LeftRing = _target.SlaveBones.LeftHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.RING)).FirstOrDefault();
            _target.SlaveBones.LeftPinky = _target.SlaveBones.LeftHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.PINKY)).FirstOrDefault();

            _target.SlaveBones.RightThumb = _target.SlaveBones.RightHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.THUMB)).FirstOrDefault();
            _target.SlaveBones.RightIndex = _target.SlaveBones.RightHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.INDEX)).FirstOrDefault();
            _target.SlaveBones.RightMiddle = _target.SlaveBones.RightHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.MIDDLE)).FirstOrDefault();
            _target.SlaveBones.RightRing = _target.SlaveBones.RightHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.RING)).FirstOrDefault();
            _target.SlaveBones.RightPinky = _target.SlaveBones.RightHand.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.PINKY)).FirstOrDefault();

            _target.SlaveBones.LeftUpperArmTwist = _target.SlaveBones.LeftHand.parent.parent.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.TWIST_ARM)).FirstOrDefault();
            _target.SlaveBones.RightUpperArmTwist = _target.SlaveBones.RightHand.parent.parent.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.TWIST_ARM)).FirstOrDefault();
            _target.SlaveBones.LeftLowerArmTwist = _target.SlaveBones.LeftHand.parent.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.TWIST_FOREARM)).FirstOrDefault();
            _target.SlaveBones.RightLoverArmTwist = _target.SlaveBones.RightHand.parent.GetComponentsInChildren<Transform>(true).Where(child => child.name.ToLower().Contains(BoneNames.TWIST_FOREARM)).FirstOrDefault();

            serializedObject.Update();
            Repaint();
        }

        private void GenerateColliders()
        {
            List<Transform> ignore = new List<Transform>();
            ignore.AddRange(_target.SlaveBones.LeftFoot.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToList());
            ignore.AddRange(_target.SlaveBones.RightFoot.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToList());
            ignore.Add(_target.SlaveBones.LeftUpperArmTwist);
            ignore.Add(_target.SlaveBones.RightUpperArmTwist);
            ignore.Add(_target.SlaveBones.LeftLowerArmTwist);
            ignore.Add(_target.SlaveBones.RightLoverArmTwist);

            var headchilds = _target.SlaveBones.Head.GetComponentsInChildren<Transform>();
            for (int i = 1; i < headchilds.Length; i++)
            {
                ignore.Add(headchilds[i]);
            }

            // sphere colliders
            EditorUtils.TryAddComponent<SphereCollider>(_target.SlaveBones.Head.gameObject);
            EditorUtils.TryAddComponent<SphereCollider>(_target.SlaveBones.LeftHand.parent.parent.parent.gameObject);
            EditorUtils.TryAddComponent<SphereCollider>(_target.SlaveBones.RightHand.parent.parent.parent.gameObject);

            // capsule colliders
            // left hand
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.LeftHand.parent.gameObject);
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.LeftHand.parent.parent.gameObject);
            // right hand
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.RightHand.parent.gameObject);
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.RightHand.parent.parent.gameObject);

            //fingers
            foreach (Transform finger in _target.SlaveBones.LeftHand.transform)
            {
                EditorUtils.TryAddComponent<CapsuleCollider>(finger.gameObject);
                EditorUtils.TryAddComponent<CapsuleCollider>(finger.GetChild(0).gameObject);
                EditorUtils.TryAddComponent<CapsuleCollider>(finger.GetChild(0).GetChild(0).gameObject);
                ignore.AddRange(finger.GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Transform>().ToList());
            }

            foreach (Transform finger in _target.SlaveBones.RightHand.transform)
            {
                EditorUtils.TryAddComponent<CapsuleCollider>(finger.gameObject);
                EditorUtils.TryAddComponent<CapsuleCollider>(finger.GetChild(0).gameObject);
                EditorUtils.TryAddComponent<CapsuleCollider>(finger.GetChild(0).GetChild(0).gameObject);
                ignore.AddRange(finger.GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Transform>().ToList());
            }

            // left leg
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.LeftFoot.parent.gameObject);
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.LeftFoot.parent.parent.gameObject);
            // right leg
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.RightFoot.parent.gameObject);
            EditorUtils.TryAddComponent<CapsuleCollider>(_target.SlaveBones.RightFoot.parent.parent.gameObject);

            // box Colliders
            var transforms = _target.SlaveBones.Hips.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].GetComponent<Collider>() == null && !ignore.Contains(transforms[i]))
                    EditorUtils.TryAddComponent<BoxCollider>(transforms[i].gameObject);
            }
        }
      
        private void GenerateJoints()
        {
            EditorUtils.TryAddComponent<Rigidbody>(_target.SlaveBones.Hips.gameObject);
            var colliders = _target.SlaveBones.Hips.GetComponentsInChildren<Collider>();
            for (int i = 1; i < colliders.Length; i++)
                EditorUtils.TryAddComponent<ConfigurableJoint>(colliders[i].gameObject);

            var joints = _target.SlaveBones.Hips.GetComponentsInChildren<ConfigurableJoint>();
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].connectedBody = joints[i].transform.parent.GetComponent<Rigidbody>();
                joints[i].xMotion = ConfigurableJointMotion.Locked;
                joints[i].zMotion = ConfigurableJointMotion.Locked;
                joints[i].yMotion = ConfigurableJointMotion.Locked;
            }
        }

        private void AssignLayersAndTags() 
        {
            var leftArm = _target.SlaveBones.LeftHand.parent.parent.GetComponentsInChildren<Transform>();
            foreach (Transform item in leftArm)
            {
                item.gameObject.layer = LayerMask.NameToLayer("LeftArm");
            }

            var rightArm = _target.SlaveBones.RightHand.parent.parent.GetComponentsInChildren<Transform>();
            foreach (Transform item in rightArm)
            {
                item.gameObject.layer = LayerMask.NameToLayer("RightArm");
            }

            var leftLeg = _target.SlaveBones.LeftFoot.parent.parent.GetComponentsInChildren<Transform>();
            foreach (Transform item in leftLeg)
            {
                item.gameObject.layer = LayerMask.NameToLayer("LeftLeg");
            }

            var rightLeg = _target.SlaveBones.RightFoot.parent.parent.GetComponentsInChildren<Transform>();
            foreach (Transform item in rightLeg)
            {
                item.gameObject.layer = LayerMask.NameToLayer("RightLeg");
            }

            var torso = _target.SlaveBones.Hips.GetComponentsInChildren<Transform>().Where(item => item.gameObject.layer == LayerMask.NameToLayer("Default"));
            foreach (Transform item in torso)
            {
                item.gameObject.layer = LayerMask.NameToLayer("Torso");
            }

            string FingerTipTag = "FingerTip";
            _target.SlaveBones.LeftThumb.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.LeftIndex.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.LeftMiddle.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.LeftRing.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.LeftPinky.GetChild(0).GetChild(0).tag = FingerTipTag;

            _target.SlaveBones.RightThumb.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.RightIndex.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.RightMiddle.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.RightRing.GetChild(0).GetChild(0).tag = FingerTipTag;
            _target.SlaveBones.RightPinky.GetChild(0).GetChild(0).tag = FingerTipTag;
        }

        private void AddTwistCorrection()
        {
            SetRotationConstraint(_target.SlaveBones.LeftLowerArmTwist, _target.SlaveBones.LeftHand);
            SetRotationConstraint(_target.SlaveBones.RightLoverArmTwist, _target.SlaveBones.RightHand);
            SetRotationConstraint(_target.SlaveBones.LeftUpperArmTwist, _target.SlaveBones.LeftHand.parent);
            SetRotationConstraint(_target.SlaveBones.RightUpperArmTwist, _target.SlaveBones.RightHand.parent);
        }

        private void SetRotationConstraint(Transform target, Transform sourceTransform) 
        {
            if (target == null)
                return;
            
            var constraint = EditorUtils.TryAddComponent<RotationConstraint>(target.gameObject);
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = sourceTransform;
            source.weight = 1;
            constraint.rotationAxis = Axis.Y;
            if (constraint.sourceCount == 0)
                constraint.AddSource(source);
            constraint.locked = true;
            constraint.weight = 0.5f;
            constraint.constraintActive = true;
        }
    }
}