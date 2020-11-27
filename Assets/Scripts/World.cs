using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Threading;

public class World : MonoBehaviour
{
    public Material material;
    public Block[] blocks;
    public Transform player;
    
}

[System.Serializable]
public class Block
{
    public string blockName;
    public bool isSolid;

    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    //how much seconds to destroy
    public float hardeness;

    //back, front, top, bottom,left,right -> order of faces being drawn for texture index always same order as in voxelData
    public int GetTexureIndex(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.LogWarning($"Error in: {MethodBase.GetCurrentMethod()}");
                return 0;
        }
    }
}

//enum values coresponds to block type ID in editor
//air blocks are assigned on the end of byte and we skip all mesh rendering and other stuff when we create them therefore they dont need blocktype or texture
[System.Serializable]
public enum BlockType
{
    BEDROCK, STONE, SAND, GRAVEL, COBBLESTONE, COAL, BRICK, WOOD, DIRT, GRASS, PLANK,FURNACE, AIR = 255
}
