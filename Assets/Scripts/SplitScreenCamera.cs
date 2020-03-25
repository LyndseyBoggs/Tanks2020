using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenCamera : MonoBehaviour
{
    private Camera playerCamera; //reference to camera object
    private TankData data; //reference to player's tank data


    // Start is called before the first frame update
    void Start()
    {
        //Get reference to the camera        
        playerCamera = GetComponent<Camera>();

        //Get reference to tank data
        data = GetComponentInParent<TankData>();

        //Identify if single player or multiplayer
        //if single player > Change nothing
        if (GameManager.instance.numberofPlayers > 1)
        {                       
            
            if (data.playerNumber == 1)
            {
                //Draw player 1 on top
                //Adjust camera height
                playerCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
            }

            else
            {
                //Draw player 2 on bottom
                //Adjust camera height
                playerCamera.rect = new Rect(0, 0, 1, 0.5f);
            }
        }
        //if multiplayer > is this player 1 or player 2?
        //if Player 1 >> Draw on Top of screen
        //if Player 2 >> Draw on Bottom of screen
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
