﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    float mouseX;
    float mouseY;
    float mouseSensitivity = 400f;
    float xRotation = 0f;
    public Transform playerBody;
   
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

    }
    //
    // Update is called once per frame
    void Update()
    {

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
