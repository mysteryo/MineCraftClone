using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionBlock : MonoBehaviour
{
    Mesh mesh;
    Vector2[] uvs;
    Vector2[] originalUvs;
    // Start is called before the first frame update
    void Awake()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        uvs = new Vector2[mesh.uv.Length];
        originalUvs = mesh.uv;
    }

    public void ShowDestruction(int textureID)
    {
        for (int i = 0; i < mesh.uv.Length; i++)
        {
            uvs[i] = new Vector2(originalUvs[i].x * .25f + textureID * .25f, originalUvs[i].y);
        }
        mesh.uv = uvs;
    }
    
}
