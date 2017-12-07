using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor {
    private int _key = 0;
    private List<LocalizedLanguage> _languages;
    private int _activeLanguage;

    public override void OnInspectorGUI() {
        var editor = LocalizationTool.Instance;
        var localText = (LocalizedText) target;

        if (editor != null) {
            _languages = editor.LocalizedLanguages;
            _activeLanguage = editor.ActiveLanguage;
        }

        var keyFromLocalText = GetKeyFromLocalText(localText);
        if (keyFromLocalText >= 0) {
            _key = keyFromLocalText;
        }

        EditorGUILayout.LabelField("Current key",
            localText.KeyPhrase != null ? localText.KeyPhrase.Key : "No key set");

        if (_languages == null) {
            EditorGUILayout.HelpBox("Open the editor to populate with keys", MessageType.Info);
            if (GUILayout.Button("Open Editor"))
            {
                LocalizationTool.Init();
            }
        }

        if (_languages == null) return;

        if (_languages.Count > 0 && _languages[_activeLanguage].KeyPhrases.Count > 0) {
            _key = EditorGUILayout.Popup("Key", _key,
                _languages[_activeLanguage].KeyPhrases.Select(x => x.Key).ToArray());
            localText.KeyPhrase = _languages[_activeLanguage].KeyPhrases[_key];
            localText.Text.text = localText.KeyPhrase.Phrase;
        }
        else {
            EditorGUILayout.HelpBox("You need at least 1 language and 1 phrase to select a key", MessageType.Info);
        }
    }

    private int GetKeyFromLocalText(LocalizedText localText) {
        if (_languages != null && 
            _languages.Count > 0 && 
            _languages[_activeLanguage].KeyPhrases.Count > 0 &&
            localText.KeyPhrase != null) {
            var currentLanguageKeyPhrases = _languages[_activeLanguage].KeyPhrases;
            // gets key from LocalizedText being inspected
            for (int i = 0; i < currentLanguageKeyPhrases.Count; i++)
            {
                if (currentLanguageKeyPhrases[i].Key == localText.KeyPhrase.Key)
                {
                    return i;
                }
            }
        }

        return -1;
    }
}