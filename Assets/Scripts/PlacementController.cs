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
    ChunkCoord destroyChunkCoord;
    Chunk c;
    DestructionBlock destroBlock;
    public SaveLoadChunkHandler saveLoadHandler;
    public LayerMask layer;
    float cooldown = .2f;
    float cooldownTimer = 0f;

    int pomX = -1;
    int pomY = -1;
    int pomZ = -1;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        saveLoadHandler = GameObject.FindObjectOfType<SaveLoadChunkHandler>();
        world = GameObject.FindObjectOfType<World>();
        ghostBlock = GameObject.Find("PlacementGhost");
        destructionGhost = GameObject.Find("DestructionGhost");
        destroBlock = destructionGhost.GetComponent<DestructionBlock>();
        ghostBlock.SetActive(false);
        destructionGhost.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (Inventory.isBlock)
        {
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance, layer))
            {
                //there is a pixel perfect position which can allow you to place block on the corner mby take care later
                Vector3 placementPoint = hit.point - mainCam.transform.forward * 0.001f;
                ghostBlock.SetActive(true);
                float x = placementPoint.x >= 0 ? (int)placementPoint.x + .5f : (int)placementPoint.x - .5f;
                float z = placementPoint.z >= 0 ? (int)placementPoint.z + .5f : (int)placementPoint.z - .5f;
                ghostBlock.transform.position = new Vector3(x, (int)placementPoint.y + .5f, z);
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
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance, layer))
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
        //int blockPosX = Mathf.Abs((int)(ghostBlock.transform.position.x % 16));
        //int blockPosY = (int)(ghostBlock.transform.position.y);
        //int blockPosZ = Mathf.Abs((int)(ghostBlock.transform.position.z % 16));

        Vector3Int blockInChunkPos = ChunkCalculations.CalculatePosInBlock(ghostBlock.transform.position);

        ChunkCoord ghostChunkCoord = ChunkCalculations.CalculateChunkFromWorldPos(ghostBlock.transform.position);
        Chunk c = VoxelData.chunkDictionary[ghostChunkCoord];
        c.voxelMap[blockInChunkPos.x, blockInChunkPos.y, blockInChunkPos.z] = (byte)Inventory.currentBlock;
        c.RegenerateChunk();
        saveLoadHandler.PrepareToSave(blockInChunkPos.x, blockInChunkPos.y, blockInChunkPos.z, ghostChunkCoord, Inventory.currentBlock);
        //Debug.Log($"ghost block pos: {ghostBlock.transform.position.x} {ghostBlock.transform.position.y} {ghostBlock.transform.position.z}");
        //Debug.Log($"Block placement = {blockInChunkPos.x} , {blockInChunkPos.y} , {blockInChunkPos.z}");
    }

    void DestroyBlock(Vector3 destroyPoint)
    {
        //int blockPosX = Mathf.Abs((int)(destroyPoint.x % 16));
        //int blockPosY = (int)(destroyPoint.y);
        //int blockPosZ = Mathf.Abs((int)(destroyPoint.z % 16));

        Vector3Int blockInChunkPos = ChunkCalculations.CalculatePosInBlock(destroyPoint);

        //Debug.Log($"{blockInChunkPos.x} , {blockInChunkPos.y} , {blockInChunkPos.z} mining this block");
        float x = destroyPoint.x >= 0 ? (int)destroyPoint.x + .5f : (int)destroyPoint.x - .5f;
        float z = destroyPoint.z >= 0 ? (int)destroyPoint.z + .5f : (int)destroyPoint.z - .5f;
        destructionGhost.transform.position = new Vector3(x, (int)destroyPoint.y + .5f, z);
        destructionGhost.SetActive(true);

        //to prevent useless initializing every frame
        if (blockInChunkPos.x != pomX || blockInChunkPos.y != pomY || blockInChunkPos.z != pomZ)
        {
            destroyChunkCoord = ChunkCalculations.CalculateChunkFromWorldPos(destroyPoint);
            c = VoxelData.chunkDictionary[destroyChunkCoord];
            pomX = blockInChunkPos.x;
            pomY = blockInChunkPos.y;
            pomZ = blockInChunkPos.z;
            timeElapsed = 0;
        }

        timeElapsed += Time.deltaTime;
        if (!PlayerInput.instantMining)
        {
            if (timeElapsed > world.blocks[c.voxelMap[blockInChunkPos.x, blockInChunkPos.y, blockInChunkPos.z]].hardeness) destroyed = true;
            ShowDestructionProgress(world.blocks[c.voxelMap[blockInChunkPos.x, blockInChunkPos.y, blockInChunkPos.z]].hardeness);
        }
        else
        {
            if(c.voxelMap[blockInChunkPos.x, blockInChunkPos.y, blockInChunkPos.z] != (byte)BlockType.BEDROCK) destroyed = true;
        }

        if (destroyed)
        {
            c.voxelMap[blockInChunkPos.x, blockInChunkPos.y, blockInChunkPos.z] = (byte)BlockType.AIR;
            c.RegenerateChunk();
            //Debug.Log($"destroy block pos: {(int)destroyPoint.x} {(int)destroyPoint.y} {(int)destroyPoint.z}");
            //Debug.Log($"Block destroyed = {blockInChunkPos.x} , {blockInChunkPos.y} , {blockInChunkPos.z}");
            ChunkCoord nextChunk;
            if (blockInChunkPos.x == 0)
            {
                nextChunk = new ChunkCoord(destroyChunkCoord.posX - 1, destroyChunkCoord.posZ);
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            if (blockInChunkPos.x == 15)
            {
                nextChunk = new ChunkCoord(destroyChunkCoord.posX + 1, destroyChunkCoord.posZ);
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            if (blockInChunkPos.z == 0)
            {
                nextChunk = new ChunkCoord(destroyChunkCoord.posX, destroyChunkCoord.posZ - 1);
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            if (blockInChunkPos.z == 15)
            {
                nextChunk = new ChunkCoord(destroyChunkCoord.posX, destroyChunkCoord.posZ + 1);
                VoxelData.chunkDictionary[nextChunk].RegenerateChunk();
            }
            ResetPoms();
            timeElapsed = 0;
            destroyed = false;
            destructionGhost.SetActive(false);
            cooldownTimer = 0;
            saveLoadHandler.PrepareToSave(blockInChunkPos.x, blockInChunkPos.y, blockInChunkPos.z, destroyChunkCoord, BlockType.AIR);
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
