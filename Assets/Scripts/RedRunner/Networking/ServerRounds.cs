using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RedRunner.Networking
{
    public class ServerRounds : MonoBehaviour
    {
        int round = 0;
        int activePlayers = 0;
        private int choosingPlayers = 0;
        private bool choosing;

        public static ServerRounds _instance;
        public static ServerRounds Instance
        {
            get
            {
                return _instance;
            }
        }
        // Start is called before the first frame update
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        // trigers round reset if not round 0 and no players alive
        public void DecrementPlayer()
        {
            if (round <= 0) return;
            activePlayers--;
            if (activePlayers == 0)
            {
                ResetRound(false);
            } else if (choosing)
            {
                DecrementChooser();
            }
        }

        public void DecrementChooser()
        {
            --choosingPlayers;
            if (choosingPlayers <= 0)
            {
                StartRound();
                choosingPlayers = 0;
            }
        }

        public void StartGame()
        {
            ScoreManager.Instance.SendPlayers();
            ResetRound(true);
        }

        public void StartRound()
        {
            RoundsManager.Instance.RpcStartRound();
            choosing = false;
        }

        public void ResetRound(bool isFirstRound)
        {
            Debug.Log("resetting round");
            round++;
            activePlayers = NetworkManager.ClientCount;
            if (activePlayers > 0) {
                choosing = true;
                choosingPlayers = activePlayers;
                RoundsManager.Instance.RpcResetRound(isFirstRound);
                StartCoroutine(OpenChoosing(3f));
			}
        }

        private IEnumerator OpenChoosing(float delay)
        {
            yield return new WaitForSeconds(delay);
            ServerSpawner.Instance.InitiateChoosing();
        }

    }
}
