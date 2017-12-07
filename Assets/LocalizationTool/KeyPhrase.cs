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

    // Created by Resharper
    protected bool Equals(KeyPhrase other) {
        return string.Equals(Key, other.Key) && string.Equals(Phrase, other.Phrase);
    }

    // Don't ask me what this returns
    public override int GetHashCode() {
        unchecked {
            return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (Phrase != null ? Phrase.GetHashCode() : 0);
        }
    }
}
