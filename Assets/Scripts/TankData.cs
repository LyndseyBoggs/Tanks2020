using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData : MonoBehaviour
{
    public float moveSpeed;               // Speed the tank moves forward and backward
    public float rotateSpeed;             // Speed at which the tank rotates
    public float moveVolume = 1;              // volume of sound the tank makes while moving (used for Noisemaker)
    public float fireRate = 1;
    public float shellForce = 1.0f;       // the force this tank applies to the shell, default to 1.0f
    public float shellDmg = 1.0f;         // amount of health the shell takes from its target enemy on hit
    public float feelDistance = 4.0f;     // Distance to look forward, begin avoidance if obstacle is hit w/ raycast

    public float timeUntilCanShoot = 1.5f;
    public bool canShoot = true;
    public GameObject lastShotBy;   //tracks the last tank to shoot this tank

    public float health; 
    public float maxHealth = 10;
    public float healthRegenPerSec = 0.5f;

    public int score = 0;                  //Q: Can I make this an unsigned int to prevent it from dropping into the neg.?
    public int playerNumber;               //Is this player 1, or player 2?
    //TODO: Assign WASD or arrow keys based on player number 

    void Start()
    {
        //Set health to max on start of game
        health = maxHealth; 
    }

    //Note: We are putting this here for class, but might be better placed into a different script (TankMortality?)
    public void TakeDamage(float damage)
    {
        //Deduct damage from health
        health -= damage;

        //If health is 0 or below, die
        if (health <= 0)
        {
            Die();
        }
    }

    //
    public void Die()
    {
        //This is what happens when the tank dies
        Destroy(this.gameObject);
    }


    //Idea: Tank Bombs (do dmg on explode, not on hit), can be kicked around like grenades. Dmg to tanks in sphere radius (use larger sphere trigger/collider)
}
