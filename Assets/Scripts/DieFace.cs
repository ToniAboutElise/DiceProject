using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class determines the result of a die depending on which face is in contact with the floor

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