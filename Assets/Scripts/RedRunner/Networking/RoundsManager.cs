using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Characters;
namespace RedRunner.Networking
{
    public class RoundsManager : Mirror.NetworkBehaviour
    {

        private static RoundsManager _local;
        public static RoundsManager Local { get { return _local; } }
        private static RoundsManager _instance;
        public static RoundsManager Instance
        {
            get
            {
                if (_local != null) return _local;
                return _instance;
            }
        }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _local = this;
        }

        // send command to server to mark player as inactive
        // finished: true if player reached final flag
        [Mirror.Command]
        public void CmdDeactivateSelf(uint netId, bool reachedEnd)
        {
            Mirror.ClientScene.FindLocalObject(netId).GetComponent<RedCharacter>().IsActiveSync = false;
            ServerRounds.Instance.DecrementPlayer();
            ScoreManager.Instance.PlayerFinished((int)netId, reachedEnd);
        }

        // receive on client to reset round
        [Mirror.ClientRpc]
        public void RpcResetRound(bool firstRound)
        {
            if (NetworkManager.IsServer) {
                foreach (var player in Object.FindObjectsOfType<RedCharacter>()) {
                    if (player != RedCharacter.Local) {
                        player.IsActiveSync = true;
                    }
                }
            }
            GameManager.Singleton.StartGame();
            GameManager.Singleton.Reset();
            if (firstRound)
            {
                //leave time for terrain to transition fro lobby to map
                StartCoroutine(StartBuildPhaseEnumerator(0.2f));
            } else
            {
                StartBuildPhase();
            }
        }

        private IEnumerator StartBuildPhaseEnumerator(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartBuildPhase();
        }

        private void StartBuildPhase()
        {
            if (!Application.isBatchMode)
            {
                GameManager.Singleton.RespawnMainCharacter();
                GameManager.Singleton.LockCharacterToStart();
            }
            UI.ScoreScreen scoreScreen = UIManager.Singleton.UISCREENS.Find(el => el.ScreenInfo == UIScreenInfo.SCORE_SCREEN) as UI.ScoreScreen;
            UIManager.Singleton.OpenScreen(scoreScreen);
            scoreScreen.SetVisible(true);
        }


        // received on client to unblock character when choosing phase is over
        [Mirror.ClientRpc]
        public void RpcStartRound()
        {
            if (!Application.isBatchMode)
            {
                GameManager.Singleton.UnlockCharacterFromStart();
                GameManager.Singleton.RespawnMainCharacter();
            }
        }

        [Mirror.Command]
        public void CmdStartGame()
        {
            ServerRounds.Instance.StartGame();
        }
    }
}
