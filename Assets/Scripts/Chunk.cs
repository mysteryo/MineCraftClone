using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    int vertexIndex = 0;
    List<Vector3> verticies = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    public int posX;
    public int posZ;
    public World world;
    public SaveLoadChunkHandler saveLoadHandler;
    public byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    private void OnEnable()
    {
        saveLoadHandler = GameObject.Find("SaveLoadHandler").GetComponent<SaveLoadChunkHandler>();
        posX = (int)transform.position.x;
        posZ = (int)transform.position.z;
        gameObject.transform.position = Vector3.zero;
        PopulateVoxelMap();
        LoadVoxelMapEdit();
        CreateMeshData();
        CreateMesh();
    }

    

    public void RegenerateChunk()
    {
        verticies.Clear();
        triangles.Clear();
        uvs.Clear();
        vertexIndex = 0;
        CreateMeshData();
        CreateMesh();
    }

    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.chunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    if (y == 0)
                    {
                        voxelMap[x, y, z] = (byte)Block.BEDROCK;
                    }
                    else if (Mathf.PerlinNoise((x + 16 * posX) * 0.05f, (z + 16 * posZ) * 0.05f) * 15 + y > VoxelData.chunkHeight * 0.7)
                    {
                        voxelMap[x, y, z] = (byte)Block.AIR;
                    }
                    else
                    {
                        voxelMap[x, y, z] = (byte)Block.GRASS;
                    }
                }
            }
        }
    }

    private void LoadVoxelMapEdit()
    {
        VectorVoxel[] vv = saveLoadHandler.LoadDataFromChunkSave(new ChunkCoord(posX, posZ));
        if (vv == null) return;
        for (int i = 0; i < vv.Length; i++)
        {
            voxelMap[vv[i].vector.x, vv[i].vector.y, vv[i].vector.z] = (byte)vv[i].blockType;
        }
    }

    //if false there is nothing to block the voxel so render the face
    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        bool b = true;

        if (y < 0 || y > VoxelData.chunkHeight - 1)
        {
            b = false;
        }
        else if (x < 0)
        {
            if (VoxelData.chunkDictionary.ContainsKey(new ChunkCoord(posX - 1, posZ)))
            {
                Chunk c = VoxelData.chunkDictionary[new ChunkCoord(posX - 1, posZ)];
                if (c.voxelMap[VoxelData.chunkWidth - 1, y, z] == 255)
                {
                    b = false;
                }
            }
            else b = false;
        }

        else if (x > VoxelData.chunkWidth - 1)
        {
            if (VoxelData.chunkDictionary.ContainsKey(new ChunkCoord(posX + 1, posZ)))
            {
                Chunk c = VoxelData.chunkDictionary[new ChunkCoord(posX + 1, posZ)];
                if (c.voxelMap[0, y, z] == 255)
                {
                    b = false;
                }
            }
            else b = false;
        }

        else if (z < 0)
        {
            if (VoxelData.chunkDictionary.ContainsKey(new ChunkCoord(posX, posZ - 1)))
            {
                Chunk c = VoxelData.chunkDictionary[new ChunkCoord(posX, posZ - 1)];
                if (c.voxelMap[x, y, VoxelData.chunkWidth - 1] == 255)
                {
                    b = false;
                }
            }
            else b = false;
        }

        else if (z > VoxelData.chunkWidth - 1)
        {
            if (VoxelData.chunkDictionary.ContainsKey(new ChunkCoord(posX, posZ + 1)))
            {
                Chunk c = VoxelData.chunkDictionary[new ChunkCoord(posX, posZ + 1)];
                if (c.voxelMap[x, y, 0] == 255)
                {
                    b = false;
                }
            }
            else b = false;
        }

        else if (voxelMap[x, y, z] == 255)
        {
            b = false;
        }
        return b;
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        if (voxelMap[(int)pos.x, (int)pos.y, (int)pos.z] == 255)
        {
            return;
        }
        Vector3 testVector = new Vector3(12, 33, 13);
        if (pos == testVector)
        {
            Debug.Log("Hit test vector");
        }
        //check all 6 faces against block next to the face
        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                BlockType bt = new BlockType();
                try
                {
                    bt = world.blockTypes[voxelMap[(int)pos.x, (int)pos.y, (int)pos.z]];

                }
                catch (System.Exception)
                {
                    Debug.LogError($"{pos.x}, {pos.y}, {pos.z}");
                    Debug.LogError($"{voxelMap[(int)pos.x, (int)pos.y, (int)pos.z]}");

                }

                Vector3 offsetPos = pos + new Vector3(16 * posX , 0, 16 * posZ );
                verticies.Add(offsetPos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 0]]);
                verticies.Add(offsetPos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 1]]);
                verticies.Add(offsetPos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 2]]);
                verticies.Add(offsetPos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 3]]);

                AddTexture(bt.GetTexureIndex(p));
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
            }
        }
    }

    void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.chunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }
    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;       //value is floored down
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks); //to get x coord we substract the Y times rows so if say textureID 6 is on second row we remove top row and pretend we are looking for ID 2 for first row
                                                                        //which is now second row
        x *= VoxelData.NormalizedBlockTextureSize;                      //textures goes from 0-1 for 4x4 texture sheet it goes 0-0.25, 0.25-0.5, 0.5-0.75, 0.75-1 so for example we need 4th column which is index 3 * 0.25 we get 0.75 which continue to 1
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;              //because top left texture has position 0 we need this substraction

        uvs.Add(new Vector2(x, y));                                     //since we have 4x4 textures we can't go (0,0)(0,1)(1,0)(1,1) as in VoxelData.voxelUvs because it will use whole texture sheet for 1 block
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));  //instead we go like (0.25,0.75) (0.25,1) (0.5,0.75) (0.75,1) so we follow same pattern but instead of 0 will use x or y and for 1 we add the normalized size which is 0.25
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }

}
