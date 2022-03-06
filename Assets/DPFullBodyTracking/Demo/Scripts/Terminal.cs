using UnityEngine;
using TMPro;

public class Terminal: MonoBehaviour
{
    public TMP_Text Text;

    public void Clear() 
    {
        Text.text = "";
    }

    public void NewText(string text) 
    {
        Clear();
        Text.text = text;
    }

    public void AddText(string text, bool withNewLine = false)
    {
        string newText = withNewLine ? "<br>" + text : text;
        Text.text += newText;
    }
}

