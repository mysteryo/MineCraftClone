using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> verticies = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    World world;
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        PopulateVoxelMap();
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
                    voxelMap[x, y, z] = 0;
                }
            }
        }
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if(x < 0 || x > VoxelData.chunkWidth -1 || y < 0 || y > VoxelData.chunkHeight - 1 || z < 0 || z > VoxelData.chunkWidth - 1)
        {
            return false;
        }
        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                verticies.Add(pos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 0]]);
                verticies.Add(pos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 1]]);
                verticies.Add(pos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 2]]);
                verticies.Add(pos + VoxelData.voxelVertecies[VoxelData.voxelTriangles[p, 3]]);

                AddTexture(6);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex +1);
                triangles.Add(vertexIndex +2);
                triangles.Add(vertexIndex +2);
                triangles.Add(vertexIndex +1);
                triangles.Add(vertexIndex +3);

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
        meshFilter.mesh = mesh;
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
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));  //instead we go like (0.25,0.75) (0.25,1) (0.5,0.75) (0.5,1) so we follow same pattern but instead of 0 will use x or y and for 1 we add the normalized size which is 0.25
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }

}
