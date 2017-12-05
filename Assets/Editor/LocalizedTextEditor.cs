using System.Linq;
using UnityEditor;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor {
    private int _key;

    public override void OnInspectorGUI()
    {
        LocalizationTool editor = (LocalizationTool)EditorWindow.GetWindow(typeof(LocalizationTool));
        var languages = editor.LocalizedLanguages.Select(x => x.LanguageName).ToArray(); 

        _key = EditorGUILayout.Popup("Key", _key, editor.LocalizedLanguages[0].KeyPhrases.Select(x => x.Key).ToArray());
        EditorGUILayout.LabelField("Test");
        LocalizedLanguage test;
        LocalizedText test2;
    }
}