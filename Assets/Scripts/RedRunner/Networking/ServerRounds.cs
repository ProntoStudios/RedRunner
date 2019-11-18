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
        private void Update()
        {
            if (Application.isBatchMode && round <= 0 && NetworkManager.ClientCount > 0)
            {
                Debug.Log("Player joined game,starting game");
                ResetRound();
            }
        }

        // trigers round reset if not round 0 and no players alive
        public void DecrementPlayer()
        {
            if (round <= 0) return;
            activePlayers--;
            if (activePlayers == 0)
            {
                ResetRound();
            }
        }

        public void StartGame()
        {
            ResetRound();
        }

        public void ResetRound()
        {
            Debug.Log("resetting round");
            round++;
            activePlayers = NetworkManager.ClientCount;
            if (activePlayers > 0) {
                RoundsManager.Instance.RpcResetRound();
                UI.ScoreScreen scoreScreen = UIManager.Singleton.UISCREENS.Find(el => el.ScreenInfo == UIScreenInfo.SCORE_SCREEN) as UI.ScoreScreen;
                UIManager.Singleton.OpenScreen(scoreScreen);
                scoreScreen.SetVisible(true);
                StartCoroutine(CloseScoreOpenChoosing(5f));
			}
        }

        private IEnumerator CloseScoreOpenChoosing(float delay)
        {
            yield return new WaitForSeconds(delay);
            UI.ScoreScreen scoreScreen = UIManager.Singleton.UISCREENS.Find(el => el.ScreenInfo == UIScreenInfo.SCORE_SCREEN) as UI.ScoreScreen;
            UIManager.Singleton.CloseScreen(scoreScreen);
            scoreScreen.SetVisible(false);
            ServerSpawner.Instance.InitiateChoosing();
        }

    }
}
