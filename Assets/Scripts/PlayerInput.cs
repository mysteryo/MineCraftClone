using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerInput : MonoBehaviour
{
    public Transform DebugMode;
    //shouldve done it from the start
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            DebugMode.gameObject.SetActive(!DebugMode.gameObject.activeSelf);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log(Application.persistentDataPath);
        if (!File.Exists(ChunkSaveObject.savePath))
        {
            Directory.CreateDirectory(ChunkSaveObject.savePath);
        }
    }
}
