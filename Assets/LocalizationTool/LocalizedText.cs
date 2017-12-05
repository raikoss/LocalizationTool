using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LocalizedText : MonoBehaviour
{
    public string Key { get; set; }
    public Text Text { get; set; }

    // Use this for initialization
    void Start()
    {
        Text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}