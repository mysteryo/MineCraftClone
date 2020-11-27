using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static bool isBlock;
    public static BlockType currentBlock;
    int index = 0;
    Color originalCol;
    List<Transform> slots = new List<Transform>();

    private void Start()
    {
        foreach (Transform item in this.transform)
        {
            slots.Add(item);
        }
        originalCol = slots[0].GetComponent<Image>().color;
        slots[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    void SetSlot(int _index)
    {
        UnhighlightCurrentSlot();
        Image im = slots[_index].GetComponent<Image>();
        im.color = new Color(1, 1, 1, 1);
        index = _index;
        if (index == 0) isBlock = false;
        else isBlock = true;
        SetBlockType();
    }

    void UnhighlightCurrentSlot()
    {
        slots[index].GetComponent<Image>().color = originalCol;
    }

    void SetBlockType()
    {
        switch (index)
        {
            case 0:
                return;
            case 1:
                currentBlock = BlockType.DIRT;
                return;
            case 2:
                currentBlock = BlockType.COBBLESTONE;
                return;
            case 3:
                currentBlock = BlockType.STONE;
                return;
            case 4:
                currentBlock = BlockType.PLANK;
                return;
            case 5:
                currentBlock = BlockType.BRICK;
                return;
            case 6:
                currentBlock = BlockType.WOOD;
                return;
            case 7:
                currentBlock = BlockType.SAND;
                return;
            case 8:
                currentBlock = BlockType.FURNACE;
                return;

            default:
                Debug.LogError("Cannot switch in inventory.cs wrong index value");
                break;
        }
    }

    private void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (index == 8) SetSlot(0);
            else SetSlot(index+1);
            
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (index == 0) SetSlot(8);
            else SetSlot(index-1);
            SetSlot(index);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SetSlot(8);
    }
}
