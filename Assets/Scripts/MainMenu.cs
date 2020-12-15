using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        PurgeDictionaries();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    void PurgeDictionaries()
    {
        VoxelData.chunkDictionary = new Dictionary<ChunkCoord, Chunk>();
        VoxelData.chunkGameobjectDictionary = new Dictionary<ChunkCoord, GameObject>();
    }
}
