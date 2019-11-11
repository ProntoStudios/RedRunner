using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;
namespace RedRunner.Networking
{
    public class ServerSpawner : MonoBehaviour
    {
        [SerializeField]
        TerrainGenerationSettings settings;
        [SerializeField]
        BlockInstantiater blockInstatiater;
        int size = 6;
        bool[] chosen;

        private static ServerSpawner _instance;

        public static ServerSpawner Instance { get { return _instance; } }
        // Start is called before the first frame update
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
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
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 1;//TerrainGenerator.ChooseFrom(settings.SpawnBlocks);
            }
            ChooserManager.Instance.RpcGetChoices(arr);
        }


        public bool ReceiveChoice(int objectId)
        {
            if (!NetworkManager.IsServer)
            {
                Debug.Log("not server");
                return false;
            }
            if (chosen[objectId])
            {
                Debug.Log("already chosen");
                return false;
            }
            chosen[objectId] = true;
            ChooserManager.Instance.RpcChoiceTaken(objectId);
            return true;
        }


        public void SpawnBlock(int objectId, Vector3 pos)
        {
            Block blockPrefab = settings.SpawnBlocks[objectId];
            blockInstatiater.GenerateBlock(blockPrefab, pos);
        }
    }
}
