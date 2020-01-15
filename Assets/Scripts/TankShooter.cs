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
        //Instantiate bullet
        GameObject myCannonBall = Instantiate(cannonBall, firePoint.transform);

        //Apply Force
        myCannonBall.SendMessage("ApplyForce", data.transform.forward); //might need to be fixed
    }
}
