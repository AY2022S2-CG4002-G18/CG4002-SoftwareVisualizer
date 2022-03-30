using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleControl : MonoBehaviour
{
    public Text text;
    public int maxLine;
    void Start()
    {
        if (!Config.CONSOLE)
        {
            gameObject.SetActive(false);
        }
    }

    public void PutText(string newText)
    {
        text.text = text.text + '\n' + newText.Trim();

        CutText();
    }

    private void CutText()
    {
        int count = 0;
        for (int i = 0; i < text.text.Length; i++)
        {
            if (text.text[i] == '\n')
            {
                count++;
            }
        }

        if (count <= maxLine) return;

        int index = text.text.IndexOf('\n');
        text.text = text.text.Remove(0, index + 1);
    }
}
