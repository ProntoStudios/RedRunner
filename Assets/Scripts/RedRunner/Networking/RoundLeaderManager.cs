using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;
using Mirror;
using RedRunner.UI;
namespace RedRunner.Networking
{
    [RequireComponent(typeof(Mirror.NetworkIdentity))]
    public class RoundLeaderManager : Mirror.NetworkBehaviour
    {
        [Mirror.Command]
        void CmdStartGame(int objectId, int type)
        {
            ServerRounds.Instance.StartGame();
        }

    }
}
