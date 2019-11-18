using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreBar : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    // set a number in range [0,1]
    public void SetPercentage(float val)
    {
        slider.value = val;
    }
}
