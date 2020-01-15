using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

//Require the components TankMotor and Tank Data for this script
//This will create the components if they do not exist on the object
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankShooter))]

public class InputController : MonoBehaviour
{
    private TankData data;
    private TankMotor motor;
    private TankShooter shooter;
    
    public enum InputScheme
    {
        WASD,
        arrowKeys
    };

    public InputScheme input = InputScheme.WASD;
    
    // Start is called before the first frame update
    void Start()
    {
        data = GetComponent<TankData>();
        motor = GetComponent<TankMotor>();
        shooter = GetComponent<TankShooter>();
    }

    // Update is called once per frame
    void Update()
    {
        //Shooter Controls
        //----------------------------------------------
        //If the player is able to shoot
        if (data.canShoot)
        {
            if (Input.GetKeyDown(KeyCode.Space)) //Will need to have a shoot button for each player
            {
                //Shoot
                shooter.Shoot();
                data.canShoot = false;
                data.timeUntilCanShoot = data.fireRate;
            }
        }

        if (data.timeUntilCanShoot > 0)
        {
            data.timeUntilCanShoot -= Time.deltaTime;
        }
        else
        {
            data.canShoot = true;
        }


        //Movement Controls
        switch (input)
        {
            case InputScheme.WASD:
                if (Input.GetKey(KeyCode.W))
                {
                    motor.Move(data.moveSpeed);
                }

                if (Input.GetKey(KeyCode.S))
                {
                    //TODO: Reverse Speed

                    motor.Move(-data.moveSpeed);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    motor.Rotate(data.rotateSpeed);
                }

                if (Input.GetKey(KeyCode.A))
                {
                    motor.Rotate(-data.rotateSpeed);
                }
                break;

            case InputScheme.arrowKeys:
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    motor.Move(data.moveSpeed);
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    //TODO: Reverse Speed

                    motor.Move(-data.moveSpeed);
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    motor.Rotate(data.rotateSpeed);
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    motor.Rotate(-data.rotateSpeed);
                }
                break;
        }
    }
}
