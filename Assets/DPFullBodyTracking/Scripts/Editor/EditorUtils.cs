using UnityEditor;
using UnityEngine;

namespace DPEditor
{
    public static class EditorUtils
    {
        public static GameObject NewGameObjectWithParent(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(obj, "");
            obj.transform.parent = parent;
            return obj;
        }

        public static T TryAddComponent<T>(GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent(out T component))
            {
                return Undo.AddComponent<T>(obj);
            }
            return component;
        }
    }
}