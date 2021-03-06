﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadChunkHandler : MonoBehaviour
{
    Transform player;
    Dictionary<ChunkCoord, Dictionary<Vector3Int, BlockType>> editedBlock = new Dictionary<ChunkCoord, Dictionary<Vector3Int, BlockType>>();
    Dictionary<ChunkCoord, Dictionary<Vector3Int, BlockType>> preparedToSave = new Dictionary<ChunkCoord, Dictionary<Vector3Int, BlockType>>();
    ChunkCoord currentChunkPos;
    ChunkCoord currentPlayerPosInChunk;
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerMove>().transform;
        currentChunkPos = ChunkCalculations.CalculateChunkFromWorldPos(player.position);
        currentPlayerPosInChunk = currentChunkPos;
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerPosInChunk = ChunkCalculations.CalculateChunkFromWorldPos(player.position);
        if (currentChunkPos.posX != currentPlayerPosInChunk.posX || currentChunkPos.posZ != currentPlayerPosInChunk.posZ)
        {
            currentChunkPos = currentPlayerPosInChunk;
            foreach (var chunkCoorditem in editedBlock)
            {
                if (Mathf.Abs(chunkCoorditem.Key.posX - currentPlayerPosInChunk.posX) > 2 || Mathf.Abs(chunkCoorditem.Key.posZ - currentPlayerPosInChunk.posZ) > 2)
                {
                    preparedToSave.Add(chunkCoorditem.Key, chunkCoorditem.Value);
                }

            }
        }
        if(editedBlock.Count > 30)
        {
            preparedToSave = editedBlock;
        }
        if (preparedToSave.Count > 0)
        {
            SaveDataToChunkSave();
        }
    }

    private void OnApplicationQuit()
    {
        preparedToSave = editedBlock;
        SaveDataToChunkSave();
    }

    private void OnDestroy()
    {
        preparedToSave = editedBlock;
        SaveDataToChunkSave();
    }

    public VectorVoxel[] LoadDataFromChunkSave(ChunkCoord chunk)
    {
        if (ChunkSaveObject.ChunkSaveExist(chunk))
        {
            ChunkSaveObject so = ChunkSaveObject.LoadChunk(ChunkSaveObject.GetPathFromChunkCoord(chunk));
            VectorVoxel[] vv = new VectorVoxel[so.vectors.Length/4];

            for (int i = 0; i < vv.Length; i++)
            {
                Vector3Int vec = new Vector3Int(so.vectors[i, 0], so.vectors[i, 1], so.vectors[i, 2]);
                BlockType bloc = (BlockType)so.vectors[i, 3];
                vv[i] = new VectorVoxel(vec, bloc);
                PrepareToSave(vec.x, vec.y, vec.z, chunk, bloc);
            }
            return vv;
        }
        return null;
    }

    void SaveDataToChunkSave()
    {
        List<ChunkCoord> toRemove = new List<ChunkCoord>();
        foreach (var chunkCoorditem in preparedToSave)
        {
            List<VectorVoxel> vvs = new List<VectorVoxel>();
            foreach (var item in chunkCoorditem.Value)
            {
                VectorVoxel vv = new VectorVoxel(item.Key, item.Value);
                vvs.Add(vv);
            }
            ChunkSaveObject so = new ChunkSaveObject(chunkCoorditem.Key, vvs.ToArray());
            ChunkSaveObject.SaveChunk(so);
            toRemove.Add(chunkCoorditem.Key);
        }
        foreach (ChunkCoord item in toRemove)
        {
            editedBlock.Remove(item);
        }
        preparedToSave.Clear();
    }

    public void UnloadEditedBlocks(ChunkCoord coord)
    {
        editedBlock.Remove(coord);
    }

    public void PrepareToSave(int x, int y, int z, ChunkCoord coord, BlockType block)
    {
        Vector3Int vec = new Vector3Int(x, y, z);
        if (editedBlock.ContainsKey(coord))
        {
            if (editedBlock[coord].ContainsKey(vec))
            {
                editedBlock[coord][vec] = block;
            }
            else
            {
                editedBlock[coord].Add(vec, block);
            }
        }
        else
        {
            Dictionary<Vector3Int, BlockType> dic = new Dictionary<Vector3Int, BlockType>();
            dic.Add(vec, block);
            editedBlock.Add(coord, dic);
        }
    }
}

public struct VectorVoxel
{
    public Vector3Int vector;
    public BlockType blockType;

    public VectorVoxel(Vector3Int _vector, BlockType _blockType)
    {
        vector = _vector;
        blockType = _blockType;
    }
}
