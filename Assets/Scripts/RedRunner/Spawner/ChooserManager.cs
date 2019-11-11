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
        private static ChooserManager _local;
        private static ChooserManager _instance;

        public static ChooserManager Local{get{return _local; } }
        public static ChooserManager Instance { get {
                if (_local != null) return _local;
                return _instance;
        } }

        private void Awake()
        {
            if(_instance = null)
            {
                _instance = this;
            }
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _local = this;
        }

        // receive block options on client
        [Mirror.ClientRpc]
        public void RpcGetChoices(int[] objects)
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
                        TrySubmitChoice(index, objects[index]);
                    }
                    );
            }
        }

        // submit a selection to the server. Could fail if someone selected the item first.
        public void TrySubmitChoice(int objectId, int type)
        {
            if (!isLocalPlayer)
            {
                Debug.LogError("can only submit choice from local player");
                return;
            }
            CmdSubmitChoice(objectId, type);
        }


        // send choice to server
        [Mirror.Command]
        void CmdSubmitChoice(int objectId, int type)
        {
            bool succeeded = ServerSpawner.Instance.ReceiveChoice(objectId);
            Debug.Log("grab " + succeeded);
            TargetSubmitChoice(connectionToClient, type, succeeded);
        }

        // runs on server, but returns results to client
        [Mirror.TargetRpc]
        public void TargetSubmitChoice(Mirror.NetworkConnection target, int type, bool succeeded)
        {
            if (!succeeded)
            {
                Debug.Log("Item grab did not succeed");
                return;
            }
            spawnerScreen.DestroyBlocks();
            UIManager.Singleton.CloseScreen(spawnerScreen);
            SpawnerManager.Instance.StartBlockPlacer(type);
        }

        // server has told client that a block has been chosen
        [Mirror.ClientRpc]
        public void RpcChoiceTaken(int objectId)
        {
            Debug.Log(objectId + " was claimed");
            spawnerScreen?.DisableBlock(objectId);
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
            ServerSpawner.Instance.SpawnBlock(objectId, pos);
        }
    }
}
