using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class LocalizedText : MonoBehaviour
{
    public KeyPhrase KeyPhrase { get; set; }
    public Text Text { get; set; }

    // Use this for initialization
    void Start()
    {
        Text = GetComponent<Text>();
        Debug.Log("LocalText start, Key: " + KeyPhrase.Key);
        //Text.text = KeyPhrase.Phrase;
    }

    // Update is called once per frame
    void Update()
    {

    }
}