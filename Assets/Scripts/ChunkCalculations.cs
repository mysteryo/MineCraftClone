using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkCalculations
{
    // Start is called before the first frame update
    public static Vector3Int CalculatePosInBlock(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x) - Mathf.FloorToInt(worldPos.x / 16) * 16;
        int y = Mathf.FloorToInt(worldPos.y);
        int z = Mathf.FloorToInt(worldPos.z) - Mathf.FloorToInt(worldPos.z / 16) * 16;

        return new Vector3Int(x, y, z);
    }

    public static ChunkCoord CalculateChunkFromWorldPos(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / 16);
        int z = Mathf.FloorToInt(worldPos.z / 16);

        return new ChunkCoord(x, z);
    }
}
