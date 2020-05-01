using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUp 
{
    public float speedMod;
    public float healthMod;
    public float maxHealthMod;
    public float fireRateMod;

    public float duration;
    public bool isPermanent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnActivate(TankData target)
    {
        //Add speed
        target.moveSpeed += speedMod;

        //Add health
        target.health += healthMod;

        //Add increase Max health
        target.maxHealth += maxHealthMod;

        //Increase Fire rate
        target.fireRate += fireRateMod;
    }

    public void OnDeactivate(TankData target)
    {
        //Subtract speed
        target.moveSpeed -= speedMod;

        //Subtract health
        target.health -= healthMod;

        //Decrease Max health
        target.maxHealth -= maxHealthMod;

        //Decrease Fire rate
        target.fireRate -= fireRateMod;
    }
}
