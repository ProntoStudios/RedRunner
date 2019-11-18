using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreScreen : MonoBehaviour
{

    private static ScoreScreen _instance;
    [SerializeField]
    private GameObject sliderPrefab;
    private Dictionary<int, UIScoreBar> scoreBars = new Dictionary<int, UIScoreBar>();
    private int curSpawnY;
    private int deltaY;

    public static ScoreScreen Instance { get { return _instance; } }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        curSpawnY = -Screen.height / 10;
        deltaY = Screen.height / 10;
        for(int i = 0; i < 5; i++)
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
        GameObject scoreObject = Instantiate(sliderPrefab, new Vector3(0,curSpawnY-deltaY,0), Quaternion.identity);
        scoreObject.transform.SetParent(gameObject.transform, false);

        curSpawnY -= deltaY;
        scoreBars[id] = scoreObject.GetComponent<UIScoreBar>();
    }

}
