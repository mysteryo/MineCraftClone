using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    public Camera mainCam;
    public float rayDistance = 6f;
    public GameObject ghostBlock;
    public GameObject destructionGhost;
    RaycastHit hit;
    bool destroyed = false;
    public World world;
    [SerializeField]
    float timeElapsed = 0;
    chunkCoord destroyChunkCoord;
    Chunk c;
    DestructionBlock destroBlock;
    public LayerMask layer;
    float cooldown = .2f;
    float cooldownTimer = 0f;

    int pomX = -1;
    int pomY = -1;
    int pomZ = -1;
    // Start is called before the first frame update
    void Start()
    {
        ghostBlock.SetActive(false);
        destructionGhost.SetActive(false);
        destroBlock = destructionGhost.GetComponent<DestructionBlock>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (Inventory.isBlock)
        {
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance,layer))
            {
                //there is a pixel perfect position which can allow you to place block on the corner mby take care later
                Vector3 placementPoint = hit.point - mainCam.transform.forward * 0.001f;
                ghostBlock.SetActive(true);
                ghostBlock.transform.position = new Vector3((int)placementPoint.x + .5f, (int)placementPoint.y + .5f, (int)placementPoint.z + .5f);
                if (Input.GetMouseButtonDown(1))
                {
                    PlaceBlock();
                }
            }
            else
            {
                ghostBlock.SetActive(false);
            }
        }
        else
        {
            ghostBlock.SetActive(false);
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance,layer))
            {
                //go a tiny bit into block
                Vector3 destroytPoint = hit.point + mainCam.transform.forward * 0.01f;
                if (Input.GetMouseButton(0) && cooldownTimer > cooldown)
                {
                    DestroyBlock(destroytPoint);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    timeElapsed = 0;
                    ResetPoms();
                    destructionGhost.SetActive(false);
                }
            }
        }
    }

    void PlaceBlock()
    {
        int blockPosX = Mathf.Abs((int)(ghostBlock.transform.position.x % 16));
        int blockPosY = (int)(ghostBlock.transform.position.y);
        int blockPosZ = Mathf.Abs((int)(ghostBlock.transform.position.z % 16));

        chunkCoord ghostChunkCoord = new chunkCoord((int)(ghostBlock.transform.position.x / 16), (int)(ghostBlock.transform.position.z / 16));
        Chunk c = VoxelData.chunkDictionary[ghostChunkCoord];
        c.voxelMap[blockPosX, blockPosY, blockPosZ] = (byte)Inventory.currentBlock;
        c.RegenerateChunk();
        Debug.Log($"ghost block pos: {ghostBlock.transform.position.x} {ghostBlock.transform.position.y} {ghostBlock.transform.position.z}");
        Debug.Log($"Block placement = {blockPosX} , {blockPosY} , {blockPosZ}");
    }

    void DestroyBlock(Vector3 destroyPoint)
    {
        int blockPosX = Mathf.Abs((int)(destroyPoint.x % 16));
        int blockPosY = (int)(destroyPoint.y);
        int blockPosZ = Mathf.Abs((int)(destroyPoint.z % 16));
        Debug.Log($"{blockPosX} , {blockPosY} , {blockPosZ} mining this block");
        destructionGhost.transform.position = new Vector3((int)destroyPoint.x +.5f, (int)destroyPoint.y +.5f, (int)destroyPoint.z +.5f);
        destructionGhost.SetActive(true);

        //to prevent useless initializing every frame
        if (blockPosX != pomX || blockPosY != pomY || blockPosZ != pomZ)
        {
            destroyChunkCoord = new chunkCoord((int)(destroyPoint.x / 16), (int)(destroyPoint.z / 16));
            c = VoxelData.chunkDictionary[destroyChunkCoord];
            pomX = blockPosX;
            pomY = blockPosY;
            pomZ = blockPosZ;
            timeElapsed = 0;
        }

        timeElapsed += Time.deltaTime;
        // you can sometimes hit an air block on the frame
        if (c.voxelMap[blockPosX, blockPosY, blockPosZ] == 255)
        {
            Debug.Log($"{blockPosX} , {blockPosY} , {blockPosZ} is an air block");
            return;
        }
        if (timeElapsed > world.blockTypes[c.voxelMap[blockPosX, blockPosY, blockPosZ]].hardeness) destroyed = true;

        ShowDestructionProgress(world.blockTypes[c.voxelMap[blockPosX, blockPosY, blockPosZ]].hardeness);

        if (destroyed)
        {
            c.voxelMap[blockPosX, blockPosY, blockPosZ] = (byte)Block.AIR;
            c.RegenerateChunk();
            Debug.Log($"destroy block pos: {(int)destroyPoint.x} {(int)destroyPoint.y} {(int)destroyPoint.z}");
            Debug.Log($"Block destroyed = {blockPosX} , {blockPosY} , {blockPosZ}");
            chunkCoord nextChunk;
            if (blockPosX == 0)
            {
                nextChunk = new chunkCoord((int)((destroyPoint.x - 1) / 16), (int)(destroyPoint.z / 16));
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            if (blockPosX == 15)
            {
                nextChunk = new chunkCoord((int)((destroyPoint.x + 1) / 16), (int)(destroyPoint.z / 16));
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            if (blockPosZ == 0)
            {
                nextChunk = new chunkCoord((int)(destroyPoint.x / 16), (int)((destroyPoint.z - 1) / 16));
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            if (blockPosZ == 15)
            {
                nextChunk = new chunkCoord((int)(destroyPoint.x / 16), (int)((destroyPoint.z + 1) / 16));
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            ResetPoms();
            timeElapsed = 0;
            destroyed = false;
            destructionGhost.SetActive(false);
            cooldownTimer = 0;
        }
    }

    void ResetPoms()
    {
        pomX = -1;
        pomY = -1;
        pomZ = -1;
    }

    void ShowDestructionProgress(float hardness)
    {
        if (timeElapsed < hardness * 0.33f) destroBlock.ShowDestruction(1);

        if (timeElapsed > hardness * 0.33f && timeElapsed < hardness * 0.66f) 
            destroBlock.ShowDestruction(2);

        if (timeElapsed > hardness * 0.66f) destroBlock.ShowDestruction(3);
    }
}
