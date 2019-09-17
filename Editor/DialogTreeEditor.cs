using UnityEditor;
using UnityEngine;

namespace Hirame.Hermes.Editor
{
    [CustomEditor (typeof (DialogTree))]
    public class DialogTreeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();
            
            using (var changeScope = new EditorGUI.ChangeCheckScope ())
            {
                if (GUILayout.Button ("Open Dialog Tree Editor") && target is DialogTree)
                {
                    DialogTreeWindow.OpenWindowWithContext (target as DialogTree);
                }

                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (serializedObject.FindProperty ("DialogEntries"), true);

                if (changeScope.changed)
                    serializedObject.ApplyModifiedProperties ();
            }
        }
    }
}