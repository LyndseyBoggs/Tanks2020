using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]

public class PowerUpController : MonoBehaviour
{
    private TankData data;
    public List<PowerUp> Powerups;   //list of all active power ups
    private List<PowerUp> expiredPowerUps;   //list of expired power ups
    
    // Start is called before the first frame update
    void Start()
    {
        //Instantiate a list to hold our active powerups
        Powerups = new List<PowerUp>();

        //Instantiate a list to hold expired powerups
        expiredPowerUps = new List<PowerUp>();

        //Get TankData component
        data = GetComponent<TankData>();
    }

    // Update is called once per frame
    void Update()
    {
        //For each powerup in the active list -> deduct time, add to expired list if expired
        foreach (PowerUp p in Powerups)
        {
            //deduct time each frame
            p.duration -= Time.deltaTime;

            //if powerup duration has expired
            if (p.duration <= 0)
            {
                //add powerup to expired powerups list
                expiredPowerUps.Add(p);
            }
        }

        //for each powerup in the expired list -> deactive powerup, remove from active list
        foreach (PowerUp p in expiredPowerUps)
        {
            //Deactivate the powerup
            p.OnDeactivate(data);

            //Remove from active list
            Powerups.Remove(p);
        }

        //Clear the Expired List, for good measure
        expiredPowerUps.Clear();
    }

    public void Add(PowerUp powerup)
    {
        //Activate the power up
        powerup.OnActivate(data);

        //If powerup is not permanent, add it to the list of powerups
        if (!powerup.isPermanent)
        {
            //Add it to our list of powerups
            Powerups.Add(powerup);
        }
        
    }
}
