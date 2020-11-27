using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class ChunkSaveObject
{
    public ChunkCoord chunk;
    public int[,] vectors;
    public BlockType blockType;

    public static string savePath = Application.persistentDataPath + "/Chunk/";

    public ChunkSaveObject(ChunkCoord _chunk, VectorVoxel[] vectorVoxel)
    {
        chunk = _chunk;
        vectors = new int[vectorVoxel.Length,4];
        for (int i = 0; i < vectorVoxel.Length; i++)
        {
            vectors[i,0] = vectorVoxel[i].vector.x;
            vectors[i,1] = vectorVoxel[i].vector.y;
            vectors[i,2] = vectorVoxel[i].vector.z;
            vectors[i,3] = (int)vectorVoxel[i].blockType;
        }
    }


    public static void SaveChunk(ChunkSaveObject so)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPathFromChunkCoord(so.chunk);
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, so);
        stream.Close();
    }

    public static ChunkSaveObject LoadChunk(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ChunkSaveObject so = formatter.Deserialize(stream) as ChunkSaveObject;
            stream.Close();
            return so;
        }
        else
        {
            Debug.Log($"No file found: {path}");
            return null;
        }
    }

    public static string GetPathFromChunkCoord(ChunkCoord coord)
    {
        return $"{savePath}{coord.posX},{coord.posZ}.bin";
    }

    public static ChunkCoord GetChunkCoordFromPath(string path)
    {
        string[] trim = path.Split('/');
        string file = trim[trim.Length - 1];
        string value = file.Split('.')[0];
        string[] coords = value.Split(',');

        int.TryParse(coords[0], out int posX);
        int.TryParse(coords[1], out int posZ);

        return new ChunkCoord(posX, posZ);
    }

    public static bool ChunkSaveExist(ChunkCoord chunk)
    {
        return File.Exists(GetPathFromChunkCoord(chunk));
    }
}
