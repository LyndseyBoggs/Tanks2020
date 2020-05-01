using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float damage;
    public float lifeTime = 3.0f;

    public GameObject ShooterTank;      //stores which tank fired this shell, value set from TankShooter when shell is instantiated

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Destroy cannonball after lifetime
        lifeTime -= Time.deltaTime; //subtract the time passed each frame

        //when life is over, destroy object
        if (lifeTime <= 0) 
        {
            Destroy(this.gameObject);
        }
    }

    //Destroy Cannonball on collision with any object
    void OnCollisionEnter(Collision col)
    {
        //Try to get TankData from collision object
        TankData otherTankData = col.gameObject.GetComponent<TankData>();
        
        //If collided with a tank, deal damage and track lastShotBy
        if (otherTankData != null)
        {
            //Set lastShotBy var on tank that was shot to be the tank that shot this cannonball
            otherTankData.lastShotBy = ShooterTank;
            
            //deal damage to the tank the cannonball collided with
            otherTankData.TakeDamage(damage);
        }
                
        
        //If collides with anything, destroy
        Destroy(this.gameObject);
    }


}
