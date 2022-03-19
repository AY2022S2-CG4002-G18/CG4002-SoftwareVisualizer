using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusColor : MonoBehaviour
{
    public Image image;

    public Color[] statusColors;

    private void Start()
    {
        TurnToColor(0);
    }

    public void TurnToColor(int index)
    {
        if (index < statusColors.Length)
        {
            image.color = statusColors[index];
        }
    }
}
