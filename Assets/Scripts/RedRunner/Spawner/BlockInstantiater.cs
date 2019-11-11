using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;
public class BlockInstantiater : MonoBehaviour
{
    public void GenerateBlock(Block blockPrefab, Vector3 pos)
    {
        Block block = Instantiate(blockPrefab, pos, Quaternion.identity);
    }
}
