using Dp;
using UnityEditor;
using UnityEngine;


namespace DPEditor
{
    [CustomEditor(typeof(AvatarManager))]
    public class AvatarManagerEditor : Editor
    {
        private AvatarManager _target;

        SerializedProperty m_SelectedAvatar;

        private void OnEnable()
        {
            _target = (AvatarManager)target;
            m_SelectedAvatar = serializedObject.FindProperty("SelectedAvatar");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(2f);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Editor option");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(m_SelectedAvatar, new GUIContent("Avatar index"), GUILayout.Height(20));
            if (GUILayout.Button("Calibrate")) 
            {
                _target.AvatarSelected(null);
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
