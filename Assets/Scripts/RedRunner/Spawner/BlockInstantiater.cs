using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;
public class BlockInstantiater : MonoBehaviour
{
    public Block GenerateBlock(Block blockPrefab, Vector3 pos)
    {
        Debug.Log("place block");

        return Instantiate(blockPrefab, pos, Quaternion.identity);
    }
}
