using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieFace : MonoBehaviour
{
    public DraggableDice draggableDice;
    public int value;
    public bool isTouchingFloor = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "floor")
        {
            draggableDice.result = value.ToString();
            isTouchingFloor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "floor")
        {
            draggableDice.result = null;
            isTouchingFloor = false;
        }
    }

}