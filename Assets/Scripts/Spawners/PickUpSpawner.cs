using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    public bool isSpawned;                  //tracks if this spawner currently holds an active pickup in the world

    public Pickup PickUp;                 //pickup to spawn

    public float RespawnTimer = 30.0f;      //designer-friendly float for seconds to wait before spawning a new pickup
    public float timeToRespawn;            //tracks time to respawn pickup based on RespawnTimer
    
    // Start is called before the first frame update
    void Awake()
    {        
        //set timeToRespawn
        timeToRespawn = RespawnTimer;
    }

    private void Start()
    {
        //instantiate pickup
        Pickup myPickup = Instantiate(PickUp, transform.position, Quaternion.identity);
        //tell pick up that this is its spawner
        myPickup.mySpawner = this;

        //set isSpawned
        isSpawned = true;
    }


    private void Update()
    {
        //If myPickup was picked up, wait for timer and then spawn new pickup
        if (!isSpawned)
        {
            //if timer is not reached, deduct time from timer
            if (timeToRespawn > 0)
            {
                timeToRespawn -= Time.deltaTime;
            }

            //else, timer is up -> spawn pick up and reset timer
            else
            {
                //instantiate pickup
                Pickup myPickup = Instantiate(PickUp, transform.position, Quaternion.identity);

                //tell pick up that this is its spawner
                myPickup.mySpawner = this;

                //Reset isSpawned
                isSpawned = true;

                //Reset timer
                timeToRespawn = RespawnTimer;
            }
        }

    }


}
