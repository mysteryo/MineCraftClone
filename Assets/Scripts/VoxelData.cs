using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{

    public static readonly int chunkWidth = 50;
    public static readonly int chunkHeight = 50;

    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1f /(float) TextureAtlasSizeInBlocks; }
    }

    //Vertex points as shown in Reference/cube points go chronologically from 0 to 7
    //order of points doesnt matter but it has to be consistent with everything else
    //as example in Reference/vertex the quad has different numbering so triangles has to have corresponding vertecies in that order
    public static readonly Vector3[] voxelVertecies = new Vector3[8]
    {
        new Vector3(0f,0f,0f),
        new Vector3(1f,0f,0f),
        new Vector3(1f,1f,0f),
        new Vector3(0f,1f,0f),
        new Vector3(0f,0f,1f),
        new Vector3(1f,0f,1f),
        new Vector3(1f,1f,1f),
        new Vector3(0f,1f,1f),
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0f,0f,-1f),
        new Vector3(0f,0f,1f),
        new Vector3(0f,1f,0f),
        new Vector3(0f,-1f,0f),
        new Vector3(-1f,0f,0f),
        new Vector3(1f,0f,0f)
    };

    public static readonly int[,] voxelTriangles = new int[6, 4]
    {
        //back, front, top, bottom,left,right -> order of faces being drawn for texture index
        //for triangle to be visible it has to have the vertecies go in clockwise order as seen in Reference/vertex
        //0 1 2 2 1 3 original indexes
        {0,3,1,2 }, //back face original for example {0, 3, 1, 1, 3, 2} -- all faces have index 1 and 2 the same thats why we can use only 4 values
        {5,6,4,7 }, //front face
        {3,7,2,6 }, //top face
        {1,5,0,4 }, //bottom face
        {4,7,0,3 }, //left face
        {1,2,5,6 }  //right face
    };

    //
    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2(0f,0f),
        new Vector2(0f,1f),
        new Vector2(1f,0f),
        new Vector2(1f,1f)
    };
}
