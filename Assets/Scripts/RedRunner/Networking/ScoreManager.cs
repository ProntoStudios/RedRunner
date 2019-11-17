using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.Networking
{
    public class ScoreManager : Mirror.NetworkBehaviour
    {

        private static ScoreManager _instance;
        public static ScoreManager Instance {get{return _instance;}}

        Dictionary<int, int> score = new Dictionary<int, int>();
        [SerializeField]
        int _scoreCap = 100;
        public int ScoreCap { get { return _scoreCap; } }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public void AddPlayer(int connectionId)
        {
            score[connectionId] = 0;
        }

        // assumes connection id already exists in score
        private void AddScore(int connectionId, int points)
        {
            score[connectionId] += points;
        }

        // connectionId: id of player that reached the end
        public void PlayerFinished(int connectionId, bool reachedEnd)
        {
            if (!reachedEnd) return;
            AddScore(connectionId, 20);
        }

        // killerId: id of player that placed obstacle that killed the current player.
        public void PlayerDied(int killerId)
        {
            AddScore(killerId, 5);
        }

        public void ResetScore()
        {
            score.Clear();
        }

    }
}
