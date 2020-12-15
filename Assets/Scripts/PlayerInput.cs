using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public Transform DebugMode;

    public static bool instantMining = false;
    //shouldve done it from the start
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            DebugMode.gameObject.SetActive(!DebugMode.gameObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            instantMining = !instantMining;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        if (!File.Exists(ChunkSaveObject.savePath))
        {
            Directory.CreateDirectory(ChunkSaveObject.savePath);
        }
    }
}
