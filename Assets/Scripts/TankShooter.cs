using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooter : MonoBehaviour
{
    private TankData data;

    public GameObject cannonBall;
    public GameObject firePoint;

    // Start is called before the first frame update
    void Start()
    {
        data = GetComponent<TankData>();
    }

    public void Shoot()
    {
        //Instantiate cannonball in world space
        GameObject myCannonBall = Instantiate(cannonBall, firePoint.transform.position, firePoint.transform.rotation);

        //Track that THIS tank created THIS cannonball
        myCannonBall.GetComponent<CannonBall>().ShooterTank = data.gameObject;

        //Apply Force to the rigidbody of the cannonball
        myCannonBall.GetComponent<Rigidbody>().AddForce(transform.forward * data.shellForce, ForceMode.Impulse); //might need to be fixed
    }
}
