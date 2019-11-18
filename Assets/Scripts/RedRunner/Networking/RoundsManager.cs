﻿using System.Collections;
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
        [Mirror.Command]
        public void CmdDeactivateSelf()
        {
            ServerRounds.Instance.DecrementPlayer();
        }

        // receive on client to reset round
        [Mirror.ClientRpc]
        public void RpcResetRound()
        {
            GameManager.Singleton.StartGame();
            GameManager.Singleton.Reset();
            if (!Application.isBatchMode) {
                GameManager.Singleton.RespawnMainCharacter();
            }
        }
    }
}
