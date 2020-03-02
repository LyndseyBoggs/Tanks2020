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
    private bool isMoving = false; //track if player is doing movement input
    
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

        isMoving = false; //player is not moving this frame unless movement input is pressed

        switch (input)
        {
            case InputScheme.WASD:
                //Forward [W]
                if (Input.GetKey(KeyCode.W))
                {
                    motor.Move(data.moveSpeed);
                    isMoving = true;
                }

                //Reverse [D]
                if (Input.GetKey(KeyCode.S))
                {
                    //TODO: Reverse Speed

                    motor.Move(-data.moveSpeed);
                    isMoving = true;
                }

                //Turn Right [D]
                if (Input.GetKey(KeyCode.D))
                {
                    motor.Rotate(data.rotateSpeed);
                }

                //Turn Left [A]
                if (Input.GetKey(KeyCode.A))
                {
                    motor.Rotate(-data.rotateSpeed);
                }

                break;

            case InputScheme.arrowKeys:
                //Forward [UP ARROW]
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    motor.Move(data.moveSpeed);
                    isMoving = true;
                }

                //Reverse [DOWN ARROW]
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    //TODO: Reverse Speed

                    motor.Move(-data.moveSpeed);
                    isMoving = true;
                }

                //Turn Right [RIGHT ARROW]
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    motor.Rotate(data.rotateSpeed);
                }

                //Turn left [LEFT ARROW]
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    motor.Rotate(-data.rotateSpeed);
                }

                break;
        }

        //If the tank is not moving, pass a speed of 0
        if (!isMoving)
        {
            //Pass a speed of 0 to the Simple Move Controller so physics are still detected
            motor.Move(0.0f);
        }
    }
}
