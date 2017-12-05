using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizedLanguage))]
public class LocalizedLanguageEditor : Editor {
    public override void OnInspectorGUI() {
        GUILayout.Space(20);

        if (GUILayout.Button("Open Editor")) {
            LocalizationTool.Init();
        }
    }
}
