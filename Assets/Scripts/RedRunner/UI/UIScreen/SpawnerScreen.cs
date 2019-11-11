using RedRunner.TerrainGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedRunner.Utilities;
using UnityEngine.Events;

namespace RedRunner.UI
{
    public class SpawnerScreen : UIScreen
    {
        [SerializeField]
        private GridLayoutGroup grid = default;
        [SerializeField]
        private GameObject buttonPrefab = default;

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
            dest.gameObject.layer = 5; // UI_LAYER

            SpriteRenderer spriteRenderer = source.GetComponent<SpriteRenderer>();
            if (null != spriteRenderer)
            {
                dest.gameObject.AddComponent<SpriteRenderer>(spriteRenderer); // copy params with reflection
                dest.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Overlay";
            }
            
            for (int i = 0; i < source.childCount; ++i)
            {
                Transform child = source.GetChild(i);
                GameObject newBlock = new GameObject(child.name, typeof(RectTransform));
                newBlock.transform.SetParent(dest);
                DFSClone(child, newBlock.transform);
            }
        }

        public void AddBlock(Block block, UnityAction callback)
        {
            GameObject newButton = Instantiate(buttonPrefab, grid.transform);
            Button button = newButton.GetComponent<Button>();

            GameObject newBlock = new GameObject(block.name, typeof(RectTransform));
            newBlock.transform.SetParent(newButton.transform);

            DFSClone(block.transform, newBlock.transform);
            newBlock.transform.localScale = newBlock.transform.localScale * 10f;

            if (null != button)
            {
                button.onClick.AddListener(callback);
            } else
            {
                Debug.LogError("Cannot find button component in choosing element.");
            }
        }
    }
}
