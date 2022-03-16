using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetSliderValue : MonoBehaviour
{
    public Slider slider;
    public Text text;

    private void Update()
    {
        SetSliderValueToText();
    }

    void SetSliderValueToText()
    {
        slider.value = int.Parse(text.text);
    }
}
