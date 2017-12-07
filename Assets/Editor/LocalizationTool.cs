using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
//using System.Windows.Forms.SaveFileDialog;

public class LocalizationTool : EditorWindow
{
    public List<LocalizedLanguage> LocalizedLanguages { get; set; }
    public int ActiveLanguage { get; set; }
    public static LocalizationTool Instance { get; private set; }

    private const string WindowName = "Localization tool";
    private const string LanguageAssetPath = "Assets/LocalizationTool/Resources/Languages";
    private int _currentLanguage;
    private bool _showNewLanguage;
    private string _newLanguageName;
    private Vector2 _scrollPos;
    
    [MenuItem("Window/" + WindowName)]
    public static void Init()
    {
        LocalizationTool window = (LocalizationTool)GetWindow(typeof(LocalizationTool), false, WindowName);
        Instance = window;
        window.Show();
    }

    void OnEnable() {
        ReloadAllLanguages();
    }

    void OnProjectChange() {
        ReloadAllLanguages();
    }
    void OnDestroy()
    {
        foreach(var language in LocalizedLanguages)
            EditorUtility.SetDirty(language);
        AssetDatabase.SaveAssets();
    }
    
    void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("ACTIVE LANGUAGE");
        ActiveLanguage = EditorGUILayout.Popup(ActiveLanguage, LocalizedLanguages.Select(x => x.LanguageName).ToArray());
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
                var newLocalizedLanguage = GetOrCreateLocalizedLanguage(_newLanguageName);
                bool createdLanguage = true;

                foreach (var language in LocalizedLanguages)
                {
                    if (language == newLocalizedLanguage)
                    {
                        createdLanguage = false;
                        break;
                    }
                }

                if (createdLanguage)
                {
                    CreateLocalizedLanguageAsset(newLocalizedLanguage);
                    ReloadAllLanguages();
                }

                _showNewLanguage = false;
            }

            if (GUILayout.Button("Close"))
            {
                _showNewLanguage = false;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        GUILayout.Label("Language");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<") && _currentLanguage > 0)
        {
            _currentLanguage--;
        }

        _currentLanguage = EditorGUILayout.Popup(_currentLanguage, LocalizedLanguages.Select(x => x.LanguageName).ToArray());

        if (GUILayout.Button(">>") && _currentLanguage < LocalizedLanguages.Count - 1)
        {
            _currentLanguage++;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (LocalizedLanguages.Count > 0) {
            var currentLanguageKeyPhrases = LocalizedLanguages[_currentLanguage].KeyPhrases;
            for (int i = 0; i < currentLanguageKeyPhrases.Count; i++) {
                currentLanguageKeyPhrases[i].Key = EditorGUILayout.TextField("Key", currentLanguageKeyPhrases[i].Key);
                foreach (var language in LocalizedLanguages) {
                    language.KeyPhrases[i].Key = currentLanguageKeyPhrases[i].Key;
                }

                currentLanguageKeyPhrases[i].Phrase =
                    EditorGUILayout.TextField("Phrase", currentLanguageKeyPhrases[i].Phrase);

                if (GUILayout.Button("Delete phrase")) {
                    var removingKey = currentLanguageKeyPhrases[i].Key;

                    foreach (var language in LocalizedLanguages) {
                        var keyPhrase = language.KeyPhrases.Find(x => x.Key == removingKey);
                        Debug.Log("Removing " + language.KeyPhrases[i].Key + " from " + language.LanguageName);
                        language.KeyPhrases.Remove(keyPhrase);
                    }
                }

                GUILayout.Space(5);
            }

            if (GUILayout.Button("New phrase")) {
                foreach (var language in LocalizedLanguages) {
                    AddNewPhrase(language);
                }
            }

            GUILayout.Space(20);
        }
        else {
            _currentLanguage = 0;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Export to JSON")) {
            var language = LocalizedLanguages[_currentLanguage];
            var json = JsonUtility.ToJson(language);
            Debug.Log(json);

            var savePath = EditorUtility.SaveFilePanel("Save location", "", language.LanguageName, "json");
            
            if (!String.IsNullOrEmpty(savePath))
                File.WriteAllBytes(savePath, System.Text.Encoding.UTF8.GetBytes(json));
        }

        if (GUILayout.Button("Load from JSON")) {
            var loadPath = EditorUtility.OpenFilePanel("Open language file", "", "json");
            if (!String.IsNullOrEmpty(loadPath)) {
                var jsonLanguage = CreateJsonLocalizedLanguage(loadPath);

                // Create the real new Language to transfer JSON phrases to
                var newLocalizedLanguage = GetOrCreateLocalizedLanguage(jsonLanguage.LanguageName);
                CreateLocalizedLanguageAsset(newLocalizedLanguage);

                AddPhrasesToAllLanguages(jsonLanguage);
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    private void ReloadAllLanguages()
    {
        LocalizedLanguages = Resources.LoadAll<LocalizedLanguage>("Languages").ToList();
    }

    private void AddNewPhrase(LocalizedLanguage language) {
        var newPhrase = new KeyPhrase("Key " + (language.KeyPhrases.Count + 1),
            language.LanguageName + " " + (language.KeyPhrases.Count + 1));
        language.KeyPhrases.Add(newPhrase);
        Debug.Log("Adding " + newPhrase.Key + " to " + language.LanguageName);
        EditorUtility.SetDirty(language);
        AssetDatabase.SaveAssets();
    }

    private LocalizedLanguage GetOrCreateLocalizedLanguage(string languageName = "") {
        foreach (var language in LocalizedLanguages) {
            if (language.LanguageName == languageName) {
                return language;
            }
        }

        var newLocalizedLanguage = CreateInstance<LocalizedLanguage>();
        newLocalizedLanguage.LanguageName = languageName;

        if (LocalizedLanguages.Count > 0)
        {
            foreach (var dummy in LocalizedLanguages[_currentLanguage].KeyPhrases)
            {
                AddNewPhrase(newLocalizedLanguage);
            }
        }

        return newLocalizedLanguage;
    }

    private void CreateLocalizedLanguageAsset(LocalizedLanguage language) {
        if (!Directory.Exists(LanguageAssetPath))
        {
            Directory.CreateDirectory(LanguageAssetPath);
        }

        if (AssetDatabase.FindAssets(language.LanguageName).Length == 0) {
            AssetDatabase.CreateAsset(language, string.Format("{0}/{1}.asset", LanguageAssetPath, language.LanguageName));
        }

        EditorUtility.SetDirty(language);
        AssetDatabase.SaveAssets();
    }

    private LocalizedLanguage CreateJsonLocalizedLanguage(string loadPath) {
        // Convert from JSON to LocalizedLanguage
        var fileBytes = File.ReadAllBytes(loadPath);
        var json = System.Text.Encoding.UTF8.GetString(fileBytes);
        Debug.Log(json);
        // Create dummy Json Language with potentially more phrases
        var jsonLanguage = GetOrCreateLocalizedLanguage();
        JsonUtility.FromJsonOverwrite(json, jsonLanguage);
        return jsonLanguage;
    }
    
    private void AddPhrasesToAllLanguages(LocalizedLanguage language) {
        // Loop through all new keyPhrases and add them to all languages
        foreach (var keyPhrase in language.KeyPhrases) {
            foreach (var localLanguage in LocalizedLanguages) {
                // If key already exists
                if (KeyExistsInLanguage(keyPhrase.Key, localLanguage)) {
                    // If it also is the same language, update the Phrase
                    if (language.LanguageName == localLanguage.LanguageName) {
                        localLanguage.KeyPhrases.Find(x => x.Key == keyPhrase.Key).Phrase = keyPhrase.Phrase;
                    }
                }
                else {
                    localLanguage.KeyPhrases.Add(keyPhrase);
                }
            }
        }
    }

    private bool KeyExistsInLanguage(string key, LocalizedLanguage language) {
        foreach (var keyPhrase in language.KeyPhrases) {
            if (key == keyPhrase.Key) {
                return true;
            }
        }

        return false;
    }
}