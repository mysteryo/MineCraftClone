using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float x;
    float z;
    public float speed = 10f;
    public CharacterController controller;
    Vector3 velocity;
    float gravity = -9.81f;
    public float mass = 1f;
    bool isGrounded;
    public LayerMask groundMask;
    public Transform groundCheck;
    float groundDistance = 0.2f;
    public float jumpHeight = 3f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity * (1/mass)); 
        }

        velocity.y += gravity * Time.deltaTime * mass;

        controller.Move(velocity * Time.deltaTime);
    }
}
