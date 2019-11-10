﻿// coding reference: https://mirror-networking.com/docs/Guides/Communications/RemoteActions.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;
using Mirror;
namespace RedRunner.Networking
{
    [RequireComponent(typeof(Mirror.NetworkIdentity))]
    public class ChooserManager : Mirror.NetworkBehaviour
    {
        [SerializeField]
        TerrainGenerationSettings settings;
        int size = 6;
        bool[] chosen;

        // must be called by the host to do anything
        public void InitiateChoosing()
        {
            if (!NetworkManager.IsServer)
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

        // receive block options on client
        [Mirror.ClientRpc]
        void RpcGetChoices(Block[] objects)
        {
            Debug.Log(objects.Length + " choices");
        }

        // submit a selection to the server. Could fail if someone selected the item first.
        public void trySubmitChoice(int objectId)
        {
            if (!isLocalPlayer)
            {
                Debug.LogError("can only submit choice from local player");
                return;
            }
            Debug.Log(CmdSubmitChoice(objectId));
        }


        // send choice to server
        [Mirror.Command]
        bool CmdSubmitChoice(int objectId)
        {
            return TargetSubmitChoice(connectionToClient, objectId);
        }

        // runs on server, but returns results to client
        [Mirror.TargetRpc]
        public bool TargetSubmitChoice(Mirror.NetworkConnection target, int objectId)
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

        // server has told client that a block has been chosen
        [Mirror.ClientRpc]
        void RpcChoiceTaken(int objectId)
        {
            Debug.Log(objectId + " was claimed");
        }
    }
}
