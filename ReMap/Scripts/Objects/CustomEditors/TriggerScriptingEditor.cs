using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(TriggerScripting))]
[CanEditMultipleObjects]
public class TriggerScriptingEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        TriggerScripting myScript = target as TriggerScripting;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Trigger_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Unity Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useWireMesh"));
        if (myScript.useWireMesh)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wireMeshSides"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("color"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(" Trigger Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Debug"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Height"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Width"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseHelperForTP"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" Enter Callback:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("EnterCallback"), new GUIContent("", ""), GUILayout.Height(200));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField(" Leave Callback:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("LeaveCallback"), new GUIContent("", ""), GUILayout.Height(200));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
