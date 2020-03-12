using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData : MonoBehaviour
{
    public float moveSpeed;          // Speed the tank moves forward and backward
    public float rotateSpeed;        // Speed at which the tank rotates
    public float fireRate = 1;
    public float shellForce = 1.0f;  // the force this tank applies to the shell, default to 1.0f
    public float shellDmg = 1.0f;    // amoun tof health the shell takes from its target enemy on hit

    public float timeUntilCanShoot = 1.5f;
    public bool canShoot = true;
    public GameObject lastShotBy;   //tracks the last tank to shoot this tank

    public float health; 
    public float maxHealth = 10;
    public float healthRegenPerSec = 0.5f;

    public int score = 0; //Q: Can I make this an unsigned int to prevent it from dropping into the neg.?

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
