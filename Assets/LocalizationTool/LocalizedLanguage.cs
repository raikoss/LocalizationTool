using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedLanguage : ScriptableObject {
    public string LanguageName;
    public List<KeyPhrase> KeyPhrases;

    public LocalizedLanguage() {
        LanguageName = "";
        KeyPhrases = new List<KeyPhrase>();
    }

    public LocalizedLanguage(string languageName, List<KeyPhrase> keyPhrases) {
        LanguageName = languageName;
        KeyPhrases = keyPhrases;
    }
}