using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    public Camera mainCam;
    public float rayDistance = 4f;
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
        

        
        if (Input.GetButtonDown("Fire2"))
        {
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayDistance))
            {
                Instantiate(ghostBlock, hit.transform.position, Quaternion.identity);
                Debug.Log($"I hit {hit.transform.gameObject.name}");

            }
        }
    }
}
