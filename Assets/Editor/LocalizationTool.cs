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
    private const string WindowName = "Localization tool";
    private const string LanguageAssetPath = "Assets/LocalizationTool/Resources/Languages";

    //TODO: Can be used later if we want to avoid loading all the languages into memory at the same time. Probably not a problem.
    //private List<string> _languages;
    public List<LocalizedLanguage> LocalizedLanguages { get; set; }

    public int ActiveLanguage { get; set; }
    public static LocalizationTool Instance { get; private set; }

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
        AssetDatabase.SaveAssets();
    }

    void ReloadAllLanguages() {
        LocalizedLanguages = Resources.LoadAll<LocalizedLanguage>("Languages").ToList();
    }
    
    void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("ACTIVE LANGUAGE");
        ActiveLanguage = EditorGUILayout.Popup(ActiveLanguage, LocalizedLanguages.Select(x => x.LanguageName).ToArray());
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
                var newLocalizedLanguage = CreateInstance<LocalizedLanguage>();
                newLocalizedLanguage.LanguageName = _newLanguageName;

                if (LocalizedLanguages.Count > 0)
                {
                    foreach (var dummy in LocalizedLanguages[_currentLanguage].KeyPhrases)
                    {
                        AddNewPhrase(newLocalizedLanguage);
                    }
                }

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

        if (LocalizedLanguages.Count > 0) { 
            var currentLanguageKeyPhrases = LocalizedLanguages[_currentLanguage].KeyPhrases;
            for (int i = 0; i < currentLanguageKeyPhrases.Count; i++)
            {
                currentLanguageKeyPhrases[i].Key = EditorGUILayout.TextField("Key", currentLanguageKeyPhrases[i].Key);
                foreach (var language in LocalizedLanguages)
                {
                    language.KeyPhrases[i].Key = currentLanguageKeyPhrases[i].Key;
                }

                currentLanguageKeyPhrases[i].Phrase = EditorGUILayout.TextField("Phrase", currentLanguageKeyPhrases[i].Phrase);

                if (GUILayout.Button("Delete phrase"))
                {
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

                AssetDatabase.SaveAssets();
            }

            GUILayout.Space(20);
        }

        GUILayout.BeginHorizontal();

        //TODO: Export til JSON
        if (GUILayout.Button("Export to JSON")) {
            var language = LocalizedLanguages[_currentLanguage];
            var json = JsonUtility.ToJson(language);
            Debug.Log(json);

            var savePath = EditorUtility.SaveFilePanel("Save location", "", language.LanguageName, "json");
            
            if (!String.IsNullOrEmpty(savePath))
                File.WriteAllBytes(savePath, System.Text.Encoding.UTF8.GetBytes(json));
        }

        //TODO: Hente fra JSON
        if (GUILayout.Button("Load from JSON")) {
            var loadPath = EditorUtility.OpenFilePanel("Open language file", "", "json");
            if (!String.IsNullOrEmpty(loadPath)) {
                var fileBytes = File.ReadAllBytes(loadPath);
                var json = System.Text.Encoding.UTF8.GetString(fileBytes);
                Debug.Log(json);
                var newLocalizedLanguage = CreateInstance<LocalizedLanguage>();
                JsonUtility.FromJsonOverwrite(json, newLocalizedLanguage); 
                AssetDatabase.CreateAsset(newLocalizedLanguage, string.Format("{0}/{1}.asset", LanguageAssetPath, newLocalizedLanguage.LanguageName));
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    private void AddNewPhrase(LocalizedLanguage language) {
        var newPhrase = new KeyPhrase("Key " + (language.KeyPhrases.Count + 1),
            language.LanguageName + " " + (language.KeyPhrases.Count + 1));
        language.KeyPhrases.Add(newPhrase);
        Debug.Log("Adding " + newPhrase.Key + " to " + language.LanguageName);
    }
}