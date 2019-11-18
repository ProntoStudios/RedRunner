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

        public void DestroyScoreBars()
        {
            foreach (KeyValuePair<int, UIScoreBar> bar in scoreBars)
            {
                Destroy(bar.Value.gameObject);
            }
            scoreBars.Clear();
        }

        public void SetVisible(bool visible)
        {
            layout.gameObject.SetActive(visible);
        }

        public static ScoreScreen Instance { get { return _instance; } }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
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

    }
}
