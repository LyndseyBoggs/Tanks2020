using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require the components TankMotor and Tank Data for this script
//This will create the components if they do not exist on the object
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankData))]

public class InputController : MonoBehaviour
{
    private TankData data;
    private TankMotor motor;

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
    }

    // Update is called once per frame
    void Update()
    {
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
