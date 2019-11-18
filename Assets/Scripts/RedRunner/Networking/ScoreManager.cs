using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.UI;
namespace RedRunner.Networking
{
    [RequireComponent(typeof(Mirror.NetworkIdentity))]
    public class ScoreManager : Mirror.NetworkBehaviour
    {

        private static ScoreManager _instance;
        public static ScoreManager Instance {get{return _instance;}}

        Dictionary<int, int> score = new Dictionary<int, int>();
        List<int> players = new List<int>();
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
            RpcGetPlayers(players.ToArray());
        }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            NetworkManager.OnClientConnected += (Mirror.NetworkConnection conn) =>
            {
                players.Add(conn.connectionId);
            };
        }

        public void AddPlayer(int connectionId)
        {
            score[connectionId] = 0;
            ScoreScreen.Instance.CreateScoreBar(connectionId);
        }

        // assumes connection id already exists in score
        [Mirror.ClientRpc]
        private void RpcAddScore(int connectionId, int points)
        {
            score[connectionId] += points;
            ScoreScreen.Instance.UpdateScore(connectionId, score[connectionId] * 1f / ScoreCap);
        }

        // connectionId: id of player that reached the end
        public void PlayerFinished(int connectionId, bool reachedEnd)
        {
            if (!reachedEnd) return;
            RpcAddScore(connectionId, 20);
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
