using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedRunner.UI
{
    public class ScoreScreen : UIScreen
    {

        private static ScoreScreen _instance;
        [SerializeField]
        private VerticalLayoutGroup layout = default;
        [SerializeField]
        private GameObject sliderPrefab;
        private Dictionary<int, UIScoreBar> scoreBars = new Dictionary<int, UIScoreBar>();

        public static ScoreScreen Instance { get { return _instance; } }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            for (int i = 0; i < 5; i++)
            {
                CreateScoreBar(i);
                UpdateScore(i, i / 5f);
            }
        }

        // newPercentage: range [0,1]
        public void UpdateScore(int id, float newPercentage)
        {
            scoreBars[id].SetPercentage(newPercentage);
        }

        public void CreateScoreBar(int id)
        {
            GameObject scoreObject = Instantiate(sliderPrefab, layout.transform);

            scoreBars[id] = scoreObject.GetComponent<UIScoreBar>();
        }

    }
}
