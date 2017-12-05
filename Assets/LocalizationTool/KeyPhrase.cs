using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyPhrase {
    public string Key;
    public string Phrase;

    public KeyPhrase(string key, string phrase) {
        Key = key;
        Phrase = phrase;
    }

    public KeyPhrase(KeyPhrase phrase) {
        Key = phrase.Key;
        Phrase = phrase.Phrase;
    }
}
