using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Awake()  
    {
        Vector3 spawnPoint = new Vector3(5 * 16, 60, 5 * 16);
        GameObject.Instantiate(player, spawnPoint, Quaternion.identity);
        Debug.Log($"AWAKE: PLAYER POS {transform.position.x},{transform.position.y},{transform.position.z}");
        
    }

    private void Start()
    {
        Debug.Log($"START: PLAYER POS {transform.position.x},{transform.position.y},{transform.position.z}");
    }
    
}
