using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Characters;
namespace RedRunner.Networking
{
    public class RoundsManager : Mirror.NetworkBehaviour
    {
        int round = 0;
        int activePlayers = 0;

        private static RoundsManager _local;
        public static RoundsManager Local { get { return _local; } }


        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _local = this;
        }

        public void ResetRound()
        {
            round++;
            activePlayers = NetworkManager.ClientCount;
            RpcResetRound();
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

        // send command to server to mark player as inactive
        [Mirror.Command]
        public void CmdDeactivateSelf()
        {
            DecrementPlayer();
        }

        // receive on client to reset round
        [Mirror.ClientRpc]
        void RpcResetRound()
        {
            GameManager.Singleton.Reset();
            GameManager.Singleton.StartGame();
            GameManager.Singleton.RespawnMainCharacter();
        }
    }
}
