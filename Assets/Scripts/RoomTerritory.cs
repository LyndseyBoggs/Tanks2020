using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTerritory : MonoBehaviour
{
    public bool isInvaded = false; // tracks if the player is currently in this territory

    private void OnTriggerEnter(Collider other)
    {
        //If Player is in the trigger box
        if (other.gameObject.tag == "Player")
        {
            //set isInvaded to true
            isInvaded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //If Player exits the trigger
        if (other.gameObject.tag == "Player")
        {
            //set isInvaded to false
            isInvaded = false;
        }
    }
}
