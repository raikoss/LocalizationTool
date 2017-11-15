using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationTool : EditorWindow
{
    private const string WindowName = "Localization tool";
    private const string LanguageAssetPath = "Assets/LocalizationTool/Resources/Languages";

    //TODO: Can be used later if we want to avoid loading all the languages into memory at the same time. Probably not a problem.
    //private List<string> _languages;
    private List<LocalizedLanguage> _localizedLanguages;
    private int _currentlySelectedLanguage = 0;
    private bool _showNewLanguage;
    private string _newLanguageName;
    private Vector2 _scrollPos;
    
    [MenuItem("Window/" + WindowName)]
    static void Init()
    {
        LocalizationTool window = (LocalizationTool)GetWindow(typeof(LocalizationTool), false, WindowName);
        window.Show();
    }

    void OnEnable() {
        ReloadAllLanguages();
    }

    void OnProjectChange() {
        ReloadAllLanguages();
    }

    void ReloadAllLanguages() {
        _localizedLanguages = Resources.LoadAll<LocalizedLanguage>("Languages").ToList();
    }

    void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("Language");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<") && _currentlySelectedLanguage > 0)
        {
            _currentlySelectedLanguage--;
        }

        _currentlySelectedLanguage = EditorGUILayout.Popup(_currentlySelectedLanguage, _localizedLanguages.Select(x => x.LanguageName).ToArray());

        if (GUILayout.Button(">>") && _currentlySelectedLanguage < _localizedLanguages.Count - 1)
        {
            _currentlySelectedLanguage++;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (GUILayout.Button("New language"))
        {
            _showNewLanguage = true;
            _newLanguageName = "";
        }

        if (_showNewLanguage)
        {
            _newLanguageName = EditorGUILayout.TextField("Name", _newLanguageName);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save language"))
            {
                var newLocalizedLanguage = ScriptableObject.CreateInstance<LocalizedLanguage>();
                newLocalizedLanguage.LanguageName = _newLanguageName;

                if (!Directory.Exists(LanguageAssetPath)) {
                    Directory.CreateDirectory(LanguageAssetPath);
                }
                AssetDatabase.CreateAsset(newLocalizedLanguage, string.Format("{0}/{1}.asset", LanguageAssetPath, _newLanguageName));
                AssetDatabase.SaveAssets();

                ReloadAllLanguages();

                _showNewLanguage = false;
            }

            if (GUILayout.Button("Close"))
            {
                _showNewLanguage = false;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        //TODO: Endre key i alle språk om en key blir endret
        //TODO: Skjule fra inspector view og ha knapp til tool
        foreach (KeyPhrase keyPhrase in _localizedLanguages[_currentlySelectedLanguage].KeyPhrases) {
            keyPhrase.Key = EditorGUILayout.TextField("Key", keyPhrase.Key);
            keyPhrase.Phrase = EditorGUILayout.TextField("Phrase", keyPhrase.Phrase);
            GUILayout.Space(5);
        }

        if (GUILayout.Button("New phrase")) {
            foreach (var language in _localizedLanguages) {
                language.KeyPhrases.Add(new KeyPhrase(GUID.Generate().ToString(), "Hello"));    
                AssetDatabase.SaveAssets();
            } 
        }

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        //TODO: Export til JSON
        if (GUILayout.Button("Export to JSON")) {
            var json = JsonUtility.ToJson(_localizedLanguages[_currentlySelectedLanguage]);
            Debug.Log(json);
        }

        //TODO: Hente fra JSON
        if (GUILayout.Button("Load from JSON"))
        {
            
        }

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }
}