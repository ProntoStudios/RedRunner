using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.UI;
using RedRunner.Characters;
namespace RedRunner.Networking
{
    [RequireComponent(typeof(Mirror.NetworkIdentity))]
    public class ScoreManager : Mirror.NetworkBehaviour
    {

        private static ScoreManager _instance;
        public static ScoreManager Instance {get{return _instance;}}

        Dictionary<int, int> score = new Dictionary<int, int>();
        [SerializeField]
        int _scoreCap = 100;
        public int ScoreCap { get { return _scoreCap; } }

        [Mirror.ClientRpc]
        private void RpcGetPlayers(int[] ids)
        {
            foreach(int id in ids)
            {
                AddPlayer(id);
            }
        }

        public void SendPlayers()
        {
            var redCharacters = FindObjectsOfType<RedCharacter>();
            int[] players = new int[redCharacters.Length];
            for(int i = 0; i < redCharacters.Length; i++)
            {
                players[i] = (int)redCharacters[i].netId;
            }
            RpcGetPlayers(players);
        }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public void AddPlayer(int id)
        {
            score[id] = 0;
            ScoreScreen.Instance.CreateScoreBar(id);
        }

        // assumes connection id already exists in score
        [Mirror.ClientRpc]
        private void RpcAddScore(int id, int points)
        {
            score[id] += points;
            ScoreScreen.Instance.UpdateScore(id, score[id] * 1f / ScoreCap);
        }

        // connectionId: id of player that reached the end
        public void PlayerFinished(int id, bool reachedEnd)
        {
			Debug.Log("finished  " + reachedEnd);
            if (!reachedEnd) return;
            RpcAddScore(id, 20);
        }

        // killerId: id of player that placed obstacle that killed the current player.
        public void PlayerDied(int killerId)
        {
            RpcAddScore(killerId, 5);
        }

        public void ResetScore()
        {
            score.Clear();
			ScoreScreen.Instance.DestroyScoreBars();

		}

    }
}
