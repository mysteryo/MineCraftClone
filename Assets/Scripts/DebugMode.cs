﻿using System.Collections;
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
    public Camera mainCam;

    TextMeshProUGUI tmpWorldPos;
    TextMeshProUGUI tmpChunk;
    TextMeshProUGUI tmpBlockInChunk;
    TextMeshProUGUI tmpWorldPositionLook;
    TextMeshProUGUI tmpChunkLook;
    TextMeshProUGUI tmpBlockInChunkLook;

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

    }

    // Update is called once per frame
    void Update()
    {
        tmpWorldPos.text = $"X:{player.transform.position.x:0.00} Y:{player.transform.position.y:0.00} Z:{player.transform.position.z:0.00}";
        tmpChunk.text = $"({(int)(player.transform.position.x / 16)} , {(int)(player.transform.position.z / 16)})";
        tmpBlockInChunk.text = $"{Mathf.Abs((int)(player.transform.position.x % 16))} , {(int)(player.transform.position.y)} , {Mathf.Abs((int)(player.transform.position.z % 16))}";
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance))
        {
            tmpWorldPositionLook.text = $"X:{hit.point.x:0.00} Y:{hit.point.y:0.00} Z:{hit.point.z:0.00}";
            tmpChunkLook.text = $"({(int)(hit.point.x / 16)} , {(int)(hit.point.z / 16)})";
            tmpBlockInChunkLook.text = $"{Mathf.Abs((int)(hit.point.x % 16))} , {Mathf.Abs((int)(hit.point.y))} , {Mathf.Abs((int)(hit.point.z % 16))}";
        }

    }
}