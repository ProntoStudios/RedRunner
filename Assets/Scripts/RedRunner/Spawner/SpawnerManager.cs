using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RedRunner.TerrainGeneration
{
    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField]
        TerrainGenerationSettings settings;
        [SerializeField]
        BitBenderGames.TouchInputController inputController;
        [SerializeField]
        BitBenderGames.MobileTouchCamera mobileTouchCamera;
        [SerializeField]
        Utilities.CameraController cameraController;
        [SerializeField]
        Camera cameraMain;
        Block activeBlock;
        bool isActive = false;

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

        void StartBlockPlacer(Block block)
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

        void StartBlockPlacer()
        {
            if (isActive)
            {
                Debug.LogError("Placer already running");
                return;
            }
            Block blockPrefab = TerrainGenerator.ChooseFrom(settings.SpawnBlocks);
            Block block = Instantiate(blockPrefab);
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

        private void Start()
        {
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
