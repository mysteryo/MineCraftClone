using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DebugMode : MonoBehaviour
{
    public Transform worldPosition;
    public Transform chunk;
    public Transform blockInChunk;
    public Transform worldPositionLook;
    public Transform chunkLook;
    public Transform blockInChunkLook;
    public Transform player;
    public Transform instantMining;
    public Transform seed;
    public Camera mainCam;

    TextMeshProUGUI tmpWorldPos;
    TextMeshProUGUI tmpChunk;
    TextMeshProUGUI tmpBlockInChunk;
    TextMeshProUGUI tmpWorldPositionLook;
    TextMeshProUGUI tmpChunkLook;
    TextMeshProUGUI tmpBlockInChunkLook;
    TextMeshProUGUI tmpInstantMining;
    TextMeshProUGUI tmpSeed;


    RaycastHit hit;
    int rayDistance = 50;
    void Start()
    {
        tmpWorldPos = worldPosition.GetComponent<TextMeshProUGUI>();
        tmpChunk = chunk.GetComponent<TextMeshProUGUI>();
        tmpBlockInChunk = blockInChunk.GetComponent<TextMeshProUGUI>();
        tmpWorldPositionLook = worldPositionLook.GetComponent<TextMeshProUGUI>();
        tmpChunkLook = chunkLook.GetComponent<TextMeshProUGUI>();
        tmpBlockInChunkLook = blockInChunkLook.GetComponent<TextMeshProUGUI>();
        tmpInstantMining = instantMining.GetComponent<TextMeshProUGUI>();
        tmpSeed = seed.GetComponent<TextMeshProUGUI>();
        player = GameObject.FindObjectOfType<PlayerMove>().transform;
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        tmpWorldPos.text = $"X:{player.transform.position.x:0.00} Y:{player.transform.position.y:0.00} Z:{player.transform.position.z:0.00}";

        tmpChunk.text = $"({Mathf.FloorToInt(player.transform.position.x / 16)} , {Mathf.FloorToInt(player.transform.position.z / 16)})";

        Vector3Int worldToChunkblock = ChunkCalculations.CalculatePosInBlock(player.transform.position);
        tmpBlockInChunk.text = $"{worldToChunkblock.x} , {worldToChunkblock.z}";

        tmpInstantMining.text = PlayerInput.instantMining.ToString();

        tmpSeed.text = StartInitializer.seed.ToString();

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance))
        {
            tmpWorldPositionLook.text = $"X:{hit.point.x:0.00} Y:{hit.point.y:0.00} Z:{hit.point.z:0.00}";

            tmpChunkLook.text = $"({Mathf.FloorToInt(hit.point.x / 16)} , {Mathf.FloorToInt(hit.point.z / 16)})";

            Vector3Int hitToChunkkBlock = ChunkCalculations.CalculatePosInBlock(hit.point);
            tmpBlockInChunkLook.text = $"{hitToChunkkBlock.x} , {hitToChunkkBlock.z}";
        }

    }
}
