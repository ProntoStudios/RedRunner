using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedRunner.Characters;
namespace RedRunner.UI {
    public class UIScoreBar : MonoBehaviour
    {
        [SerializeField]
        Slider slider;
        [SerializeField]
        Text textBox;

        public void SetId(int id)
        {
            string text = "Player " + id;
            if (RedCharacter.Local.netId == id)
            {
                text = "YOU";
            }
            textBox.text = text;
    }
        // set a number in range [0,1]
        public void SetPercentage(float val)
        {
            slider.value = val;
        }
    }
}