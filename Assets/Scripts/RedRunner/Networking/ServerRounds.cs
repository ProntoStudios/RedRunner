using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RedRunner.Networking
{
    public class ServerRounds : MonoBehaviour
    {
        int round = 0;
        int activePlayers = 0;

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

        public void DecrementPlayer()
        {
            activePlayers--;
        }

        public void ResetRound()
        {
            Debug.Log("resetting round");
            round++;
            activePlayers = NetworkManager.ClientCount;
            if (activePlayers > 0) {
                RoundsManager.Instance.RpcResetRound();
				ServerSpawner.Instance.InitiateChoosing();
			}
        }

    }
}
