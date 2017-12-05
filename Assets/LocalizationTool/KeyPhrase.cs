using System;
using Newtonsoft.Json;

[Serializable]
public class KeyPhrase {
    public string Key;
    public string Phrase;

    [JsonConstructor]
    public KeyPhrase(string key, string phrase) {
        Key = key;
        Phrase = phrase;
    }

    public KeyPhrase(KeyPhrase phrase) {
        Key = phrase.Key;
        Phrase = phrase.Phrase;
    }
}
