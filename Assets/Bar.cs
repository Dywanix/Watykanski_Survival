using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Slider slider;
    public Image fill;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
