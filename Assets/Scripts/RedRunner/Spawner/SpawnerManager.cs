using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Networking;
namespace RedRunner.TerrainGeneration
{
    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField]
        TerrainGenerationSettings settings;
        [SerializeField]
        GameObject spawnerButton;
        [SerializeField]
        BitBenderGames.TouchInputController inputController;
        [SerializeField]
        BitBenderGames.MobileTouchCamera mobileTouchCamera;
        [SerializeField]
        Utilities.CameraController cameraController;
        [SerializeField]
        Camera cameraMain;
        Block activeBlock;
        int blockId;
        bool isActive = false;

        private static SpawnerManager _instance;

        public static SpawnerManager Instance { get { return _instance; } }
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }
        void EnableScrolling()
        {
            cameraController.enabled = false;
            inputController.enabled = true;
            mobileTouchCamera.enabled = true;
        }

        void DisableScrolling()
        {
            mobileTouchCamera.enabled = false;
            inputController.enabled = false;
            cameraController.enabled = true;
        }

        public void StartBlockPlacer(Block block)
        {
            if (isActive)
            {
                Debug.LogError("Placer already running");
                return;
            }
            activeBlock = block;
            EnableScrolling();
            isActive = true;
        }

        public void FinishBlockPlacer()
        {
            spawnerButton.SetActive(false);
            DisableScrolling();
            Vector3 pos = activeBlock.transform.position;
            Destroy(activeBlock.gameObject);
            activeBlock = null;
            isActive = false;
            ChooserManager.Local.SubmitPosition(blockId, pos);
        }

        public void StartBlockPlacer(int id)
        {
            if (isActive)
            {
                Debug.LogError("Placer already running");
                return;
            }
            if(id < 0 || id > settings.SpawnBlocks.Length)
            {
                Debug.LogError("Placer id " + id + " inactive");
                return;
            }

            spawnerButton.SetActive(true);
            Block blockPrefab = settings.SpawnBlocks[id];
            Block block = Instantiate(blockPrefab);
            blockId = id;
            StartBlockPlacer(block);
        }

        // Runs a frame of block placer, which is assumed to be active.
        void RunBlockPlacer()
        {
            if(activeBlock == null)
            {
                Debug.LogError("Block Spawner has no active block");
                return;
            }
            activeBlock.transform.position = new Vector3(cameraMain.transform.position.x, cameraMain.transform.position.y, activeBlock.transform.position.z);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isActive)
            {
                RunBlockPlacer();
            }
        }
    }
}
