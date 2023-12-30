using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(SpawnPointScript))]
[CanEditMultipleObjects]
public class SpawnPointScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        SpawnPointScript myScript = target as SpawnPointScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Spawn_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" No Settings", CustomEditorStyle.LabelStyle);

        serializedObject.ApplyModifiedProperties();
    }
}
