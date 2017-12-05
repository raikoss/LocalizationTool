using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor {
    private int _key;
    private List<LocalizedLanguage> _languages;
    private int _activeLanguage;

    public override void OnInspectorGUI() {
        LocalizationTool editor = LocalizationTool.Instance;
        LocalizedText localText = (LocalizedText) target;

        if (editor != null) {
            _languages = editor.LocalizedLanguages;
            _activeLanguage = editor.ActiveLanguage;
            //localText.Text = localText.gameObject.GetComponent<Text>(); 
        }

        if (_languages != null && _languages.Count > 0 && _languages[_activeLanguage].KeyPhrases.Count > 0) {
            _key = EditorGUILayout.Popup("Key", _key,
                _languages[0].KeyPhrases.Select(x => x.Key).ToArray());
            var activeLanguage = _languages[_activeLanguage];
            var text = activeLanguage.KeyPhrases[_key].Phrase;
            localText.Text.text = text;
        }
        else if (editor == null) {
            EditorGUILayout.HelpBox("Open the editor to populate with keys", MessageType.Info);
            if (GUILayout.Button("Open Editor")) {
                LocalizationTool.Init();
            }
        }
    }
}