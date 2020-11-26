using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class ChunkSaveObject
{
    int x;
    int y;
    int z;
    ChunkCoord chunkCoord;
    Block block;

    public static string savePath = Application.persistentDataPath + "/Chunk/";

    public ChunkSaveObject(int _x, int _y, int _z, ChunkCoord _chunkCoord, Block _block)
    {
        x = _x;
        y = _y;
        z = _z;
        chunkCoord = _chunkCoord;
        block = _block;
    }


    public static void SaveChunk(ChunkSaveObject so)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPathFromChunkCoord(so.chunkCoord);
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
}
