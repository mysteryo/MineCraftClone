using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

public class GenerateChunks : MonoBehaviour
{
    public Transform player;
    public World world;
    Queue<ChunkCoord> toGenerate = new Queue<ChunkCoord>();
    HashSet<ChunkCoord> toGenerateUniques = new HashSet<ChunkCoord>();
    Queue<ChunkCoord> toDestroy = new Queue<ChunkCoord>();
    HashSet<ChunkCoord> toDestroyUniques = new HashSet<ChunkCoord>();
    ChunkCoord currentChunkPos = new ChunkCoord(0, 0);
    ChunkCoord currentPlayerPosInChunk;
    int viewRange = 5;
    public GameObject chunk;
    public SaveLoadChunkHandler saveLoadHandler;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                SpawnChunk(new ChunkCoord(i, j));
            }
        }
        Vector3 spawnPoint = new Vector3(5 * VoxelData.chunkWidth, 60, 5 * VoxelData.chunkWidth);
        player.position = spawnPoint;
    }

    private void Update()
    {
        UnloadChunks();
    }

    //from project settings the fixed update is set to 0.05 seconds ie. 20 tics per second
    //generating  1 chunk from queue every 0.05 seconds should offload some work when processing giant squares of new chunks at once
    //after trying it seems to not have impact on basic physics in game
    //if weird things wiith physics will occur probably switch to courutine

    private void FixedUpdate()
    {
        currentPlayerPosInChunk = ChunkCalculations.CalculateChunkFromWorldPos(player.position);
        if (currentChunkPos.posX != currentPlayerPosInChunk.posX || currentChunkPos.posZ != currentPlayerPosInChunk.posZ)
        {
            for (int i = -5; i <= 5; i++)
            {
                for (int j = -5; j <= 5; j++)
                {
                    if (!VoxelData.chunkDictionary.ContainsKey(new ChunkCoord((int)(player.position.x / 15 + i), (int)(player.position.z / 15 + j))))
                    {
                        int x = (int)(player.position.x / 15 + i);
                        int z = (int)(player.position.z / 15 + j);
                        if (toGenerateUniques.Add(new ChunkCoord(x, z)))
                        {
                            toGenerate.Enqueue(new ChunkCoord(x, z));
                        }
                    }
                }
            }
            currentChunkPos = currentPlayerPosInChunk;
        }
        if (toGenerate.Count > 0)
        {
            toGenerateUniques.Remove(toGenerate.Peek());
            SpawnChunk(toGenerate.Dequeue());
        }
    }

    void SpawnChunk(ChunkCoord coord)
    {
        int posX = coord.posX;
        int posZ = coord.posZ;

        GameObject chunkGO = Instantiate(chunk, new Vector3(posX, 0, posZ), Quaternion.identity, world.gameObject.transform);
        chunkGO.name = $"Chunk({posX},{posZ})";

        Chunk chunkObj = chunkGO.GetComponent<Chunk>();
        VoxelData.chunkDictionary.Add(coord, chunkObj);
    }

    void UnloadChunks()
    {
        List<ChunkCoord> toDestroy = new List<ChunkCoord>();
        foreach (var item in VoxelData.chunkDictionary)
        {
            if ((int)(player.position.x / 15) - viewRange - 2 > item.Key.posX || (int)(player.position.x / 15) + viewRange + 2 < item.Key.posX
                || (int)(player.position.z / 15) - viewRange - 2 > item.Key.posZ || (int)(player.position.z / 15) + viewRange + 2 < item.Key.posZ)
            {
                toDestroy.Add(item.Key);
            }
        }
        if (toDestroy.Count > 0)
        {
            GameObject.Destroy(VoxelData.chunkDictionary[toDestroy[0]].gameObject);
            VoxelData.chunkDictionary.Remove(toDestroy[0]);
            saveLoadHandler.UnloadEditedBlocks(toDestroy[0]);
            toDestroy.RemoveAt(0);
        }
    }
}
