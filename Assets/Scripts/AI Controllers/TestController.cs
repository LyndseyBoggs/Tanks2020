using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require must be placed before class declaration
//This will add these components to the game object if they are not otherwise found on the object
[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]

public class TestController : MonoBehaviour
{
    private TankMotor motor;
    private TankData data;
    
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<TankMotor>();
        data = GetComponent<TankData>();
    }

    // Update is called once per frame
    void Update()
    {
        motor.Move(data.moveSpeed);
        motor.Rotate(data.rotateSpeed);
    }
}
