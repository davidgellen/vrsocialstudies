using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Dp.Rig;

namespace DPEditor
{
    [CustomEditor(typeof(VRRig))]
    public partial class VRRigEditor : Editor
    {
        private VRRig _target;
        private Transform _rigExists;

        SerializedProperty m_Parent;
        SerializedProperty m_AvatarNeckBone;
        SerializedProperty m_AvatarHegiht;
        SerializedProperty m_Calibrated;

        SerializedObject m_Target;
        SerializedProperty m_BonesData;
        SerializedProperty m_Model;
        SerializedProperty m_Head;
        SerializedProperty m_Hips;
        SerializedProperty m_LeftArm;
        SerializedProperty m_RightArm;
        SerializedProperty m_LeftLeg;
        SerializedProperty m_RightLeg;
        SerializedProperty m_LeftHand;
        SerializedProperty m_RightHand;

        private void OnEnable()
        {
            _target = (VRRig)target;
            m_Target = new SerializedObject(target);
            _rigExists = _target.gameObject.transform.Find(RIG_NAME);

            m_BonesData = m_Target.FindProperty("BonesData");
            m_Model = m_Target.FindProperty("Model");

            m_Parent = m_Model.FindPropertyRelative("_cameraRig");
            m_AvatarNeckBone = m_Model.FindPropertyRelative("_avatarNeck");
            m_AvatarHegiht = m_Model.FindPropertyRelative("_avatarHeight");
            m_Calibrated = m_Target.FindProperty("Calibrated");

            m_Head = m_Model.FindPropertyRelative("_head");
            m_Hips = m_Model.FindPropertyRelative("_hips");
            m_LeftArm = m_Model.FindPropertyRelative("_leftArm");
            m_RightArm = m_Model.FindPropertyRelative("_rightArm");
            m_LeftLeg = m_Model.FindPropertyRelative("_leftLeg");
            m_RightLeg = m_Model.FindPropertyRelative("_rightLeg");
            m_LeftHand = m_Model.FindPropertyRelative("_leftHand");
            m_RightHand = m_Model.FindPropertyRelative("_rightHand");
        }

        public override void OnInspectorGUI()
        {
            if (_rigExists == null)
                ConstraintBuilderInspectorGUI();
            else
                VRRigOnInspectorGUI();
        }

        private void VRRigOnInspectorGUI() 
        {
            EditorGUILayout.BeginVertical(GUI.skin.box); EditorGUI.indentLevel++;
            {
                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
                EditorGUILayout.LabelField("Requirements", style, GUILayout.ExpandWidth(true));
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_Parent, true);
                EditorGUILayout.PropertyField(m_AvatarNeckBone, true);
                EditorGUILayout.PropertyField(m_AvatarHegiht, true);
                //EditorGUILayout.PropertyField(m_Calibrated, true);
            }
            EditorGUI.indentLevel--; EditorGUILayout.EndVertical();


            DrawBox(m_Head);
            DrawBox(m_Hips);
            DrawBox(m_LeftArm);
            DrawBox(m_RightArm);
            DrawBox(m_LeftLeg);
            DrawBox(m_RightLeg);
            DrawBox(m_LeftHand);
            DrawBox(m_RightHand);

            m_Target.ApplyModifiedProperties();
            m_Target.UpdateIfRequiredOrScript();
        }

        void DrawBox(SerializedProperty property) 
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box); EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(property, true);
            }
            EditorGUI.indentLevel--; EditorGUILayout.EndHorizontal();
        }
    }
}