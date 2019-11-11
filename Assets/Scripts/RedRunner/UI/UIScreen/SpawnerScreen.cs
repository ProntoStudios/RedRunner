using RedRunner.TerrainGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedRunner.UI
{
    public class SpawnerScreen : UIScreen
    {
        [SerializeField]
        private GridLayoutGroup grid = default;

        private void Start()
        { 

        }

        public override void UpdateScreenStatus(bool open)
        {
            base.UpdateScreenStatus(open);
        }

        public void AddBlock(Block block)
        {
            GameObject newBlock = new GameObject(block.name, typeof(RectTransform));
            newBlock.transform.SetParent(grid.transform, false);
        }
    }
}
