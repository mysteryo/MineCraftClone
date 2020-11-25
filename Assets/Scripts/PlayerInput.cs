using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
