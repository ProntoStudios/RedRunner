using RedRunner.TerrainGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedRunner.Utilities;

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

        private void DFSClone(Transform source, Transform dest)
        {
            dest.localScale = source.localScale;
            dest.localPosition = source.localPosition;
            dest.localRotation = source.localRotation;
            SpriteRenderer spriteRenderer = source.GetComponent<SpriteRenderer>();
            if (null != spriteRenderer)
            {
                dest.gameObject.AddComponent<SpriteRenderer>(spriteRenderer); // copy params with reflection
            }
            
            for (int i = 0; i < source.childCount; ++i)
            {
                Transform child = source.GetChild(i);
                GameObject newBlock = new GameObject(child.name, typeof(RectTransform));
                newBlock.transform.SetParent(dest);
                DFSClone(child, newBlock.transform);
            }
        }

        public void AddBlock(Block block)
        {
            GameObject newBlock = new GameObject(block.name, typeof(RectTransform));
            newBlock.transform.SetParent(grid.transform, false);
            DFSClone(block.transform, newBlock.transform);
            newBlock.transform.localScale = newBlock.transform.localScale * 15f;
        }
    }
}
