// coding reference: https://mirror-networking.com/docs/Guides/Communications/RemoteActions.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RedRunner.TerrainGeneration;
namespace RedRunner.Networking
{
    [RequireComponent(typeof(Mirror.NetworkIdentity))]
    public class ChooserManager : NetworkBehaviour
    {
        [SerializeField]
        TerrainGenerationSettings settings;
        int size = 6;
        bool[] chosen;

        // must be called by the host to do anything
        public void InitiateChoosing()
        {
            if (!isServer)
            {
                Debug.LogError("can only initialize block chooser from host");
                return;
            }
            Block[] arr = new Block[size];
            chosen = new bool[size];
            for(int i = 0; i < arr.Length; i++)
            {
                arr[i] = TerrainGenerator.ChooseFrom(settings.SpawnBlocks);
            }
            RpcGetChoices(arr);
        }

        // submit a selection to the server. Could fail if someone selected the item first.
        public bool trySubmitChoice(int objectId)
        {
            if (!isLocalPlayer)
            {
                Debug.LogError("can only submit choice from local player");
                return false;
            }
            return CmdSubmitChoice(objectId);
        }


        // send choice to server
        [Command]
        bool CmdSubmitChoice(int objectId)
        {
            return TargetSubmitChoice(connectionToClient, objectId);
        }

        // runs on server, but returns results to client
        [TargetRpc]
        public bool TargetSubmitChoice(NetworkConnection target, int objectId)
        {
            if(objectId < 0 || objectId >= chosen.Length)
            {
                Debug.LogError("Object id invalid");
                return false;
            }
            if (chosen[objectId]) return false;
            chosen[objectId] = true;
            RpcChoiceTaken(objectId);
            return true; 
        }

        // receive block options on client
        [ClientRpc]
        void RpcGetChoices(Block[] objects)
        {
            Debug.Log(objects.Length + " choices");
        }

        // server has told client that a block has been chosen
        [ClientRpc]
        void RpcChoiceTaken(int objectId)
        {
            Debug.Log(objectId + " was claimed");
        }
    }
}
