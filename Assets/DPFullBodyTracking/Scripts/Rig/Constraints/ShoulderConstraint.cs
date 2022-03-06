using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Shoulder IK Constraint")]
public class ShoulderConstraint : RigConstraint<
    ShoulderIKConstraintJob,
    ChainIKConstraintData,
    ChainIKConstraintJobBinder<ChainIKConstraintData>
    >
{
#if UNITY_EDITOR
#pragma warning disable 0414
    [NotKeyable, SerializeField, HideInInspector] bool m_SourceObjectsGUIToggle;
    [NotKeyable, SerializeField, HideInInspector] bool m_SettingsGUIToggle;
#endif
}