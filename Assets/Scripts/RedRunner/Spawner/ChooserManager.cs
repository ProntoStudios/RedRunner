// coding reference: https://mirror-networking.com/docs/Guides/Communications/RemoteActions.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;
using Mirror;
using RedRunner.UI;

namespace RedRunner.Networking
{
    [RequireComponent(typeof(Mirror.NetworkIdentity))]
    public class ChooserManager : Mirror.NetworkBehaviour
    {
        [SerializeField]
        TerrainGenerationSettings settings;
        [SerializeField]
        BlockInstantiater blockInstatiater;
        int size = 6;
        bool[] chosen;
        private static ChooserManager _local;
        private SpawnerScreen spawnerScreen; 

        public static ChooserManager Local{get{return _local; }}


        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _local = this;
        }

        // must be called by the host to do anything
        public void InitiateChoosing()
        {
            if (!NetworkManager.IsServer)
            {
                Debug.LogError("can only initialize block chooser from host");
                return;
            }
            int[] arr = new int[size];
            chosen = new bool[size];

            for(int i = 0; i < arr.Length; i++)
            {
                arr[i] = Random.Range(0, settings.SpawnBlocks.Length);
            }
            RpcGetChoices(arr);
        }

        // receive block options on client
        [Mirror.ClientRpc]
        void RpcGetChoices(int[] objects)
        {
            Debug.Log(objects.Length + " choices");
            spawnerScreen = UIManager.Singleton.UISCREENS.Find(el => el.ScreenInfo == UIScreenInfo.SPAWNER_SCREEN) as SpawnerScreen;
            UIManager.Singleton.OpenScreen(spawnerScreen);
            //GameManager.Singleton.StopGame();

            Block[] blocks = new Block[objects.Length];
            for (int i = 0; i < objects.Length; ++i)
            {
                var index = i; // to capture this instance
                spawnerScreen.AddBlock(settings.SpawnBlocks[objects[i]], index, 
                    () => 
                    {
                        TrySubmitChoice(index);
                    }
                    );
            }
        }

        // submit a selection to the server. Could fail if someone selected the item first.
        public void TrySubmitChoice(int objectId)
        {
            if (!isLocalPlayer)
            {
                Debug.LogError("can only submit choice from local player");
                return;
            }
            CmdSubmitChoice(objectId);
        }


        // send choice to server
        [Mirror.Command]
        void CmdSubmitChoice(int objectId)
        {
            bool succeeded = Local.ReceiveChoice(objectId);
            Debug.Log("grab " + succeeded);
            TargetSubmitChoice(connectionToClient, objectId, succeeded);
        }

        public bool ReceiveChoice(int objectId)
        {
            if (!NetworkManager.IsServer)
            {
                Debug.Log("not server");
                return false;
            }
            if (chosen[objectId]){
                Debug.Log("already chosen");
                return false;
            }
            chosen[objectId] = true;
            RpcChoiceTaken(objectId);
            return true;
        }

        // runs on server, but returns results to client
        [Mirror.TargetRpc]
        public void TargetSubmitChoice(Mirror.NetworkConnection target, int objectId, bool succeeded)
        {
            if (!succeeded)
            {
                Debug.Log("Item grab did not succeed");
                return;
            }
            spawnerScreen.DestroyBlocks();
            UIManager.Singleton.CloseScreen(spawnerScreen);
            SpawnerManager.Instance.StartBlockPlacer(objectId);
        }

        // server has told client that a block has been chosen
        [Mirror.ClientRpc]
        void RpcChoiceTaken(int objectId)
        {
            Debug.Log(objectId + " was claimed");
        }

        // send block location to server
        public void SubmitPosition(int objectId, Vector3 pos)
        {
            if (!isLocalPlayer)
            {
                Debug.LogError("can only submit choice from local player");
                return;
            }
            CmdSubmitPosition(objectId, pos);
        }

        // send block location to server
        // TODO(wilson): taking in id is hacky, should be fixed so server knows based on client id.
        [Mirror.Command]
        void CmdSubmitPosition(int objectId, Vector3 pos)
        {
            Block blockPrefab = settings.SpawnBlocks[objectId];
            blockInstatiater.GenerateBlock(blockPrefab, pos);
        }
    }
}
