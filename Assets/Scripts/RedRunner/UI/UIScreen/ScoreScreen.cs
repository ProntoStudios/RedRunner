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
        }
        public void Temp()
        {
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
            UIScoreBar score = scoreObject.GetComponent<UIScoreBar>();
            score.SetId(id);
            scoreBars[id] = score;
        }


        /*
        public void StartRound()
        {
            RedRunner.UI.ScoreScreen scoreScreen = RedRunner.UIManager.Singleton.UISCREENS.Find(el => el.ScreenInfo == RedRunner.UIScreenInfo.SCORE_SCREEN) as RedRunner.UI.ScoreScreen;
            RedRunner.UIManager.Singleton.OpenScreen(scoreScreen);
            scoreScreen.Temp();
        }
        */

    }
}
