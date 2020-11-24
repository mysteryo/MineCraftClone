using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    public Camera mainCam;
    public float rayDistance = 6f;
    public GameObject ghostBlock;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        ghostBlock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance))
        {
            //there is a pixel perfect position which can allow you to place block on the corner mby take care later
            Vector3 placementPoint = hit.point - mainCam.transform.forward * 0.001f;
            ghostBlock.SetActive(true);
            ghostBlock.transform.position = new Vector3((int)placementPoint.x + .5f, (int)placementPoint.y + .5f, (int)placementPoint.z + .5f);
            if (Input.GetMouseButtonDown(0))
            {
                PlaceBlock();
            }
        }
        else
        {
            ghostBlock.SetActive(false);
        }
    }

    void PlaceBlock()
    {
        int blockPosX = (int)(ghostBlock.transform.position.x % 16);
        int blockPosY = (int)(ghostBlock.transform.position.y);
        int blockPosZ = (int)(ghostBlock.transform.position.z % 16);

        chunkCoord ghostChunkCoord = new chunkCoord((int)(ghostBlock.transform.position.x / 16), (int)(ghostBlock.transform.position.z / 16));
        Chunk c = VoxelData.chunkDictionary[ghostChunkCoord];

        c.voxelMap[blockPosX, blockPosY, blockPosZ] = 2;
        c.RegenerateChunk();
        Debug.Log($"ghost block pos: {ghostBlock.transform.position.x} {ghostBlock.transform.position.y} {ghostBlock.transform.position.z}");
        Debug.Log($"Block placement = {blockPosX} , {blockPosY} , {blockPosZ}");
    }
}
