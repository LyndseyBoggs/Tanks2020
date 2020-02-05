using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]
public class SampleAIControllerThree : MonoBehaviour
{
    public Transform target;
    private TankMotor motor;
    private TankData data;
    private Transform tf;

    //private int avoidanceStage = 0;

    public enum AvoidanceStage
    {
        None,
        Rotate,
        Move
    };

    public AvoidanceStage avoidanceStage;
    public float avoidanceTime = 2.0f;

    private float exitTime;
    public enum AttackMode
    {
        Chase
    };

    public AttackMode attackMode;

    
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<TankMotor>();
        data = GetComponent<TankData>();
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackMode == AttackMode.Chase)
        {
            if (avoidanceStage != AvoidanceStage.None)
            {
                Avoid();
            }
            else
            {
                Chase();
            }

        }
    }

    //Chase a target position
    private void Chase()
    {
        //Face my target
        motor.RotateTowards(target.position, data.rotateSpeed);

        //If I can move, move forward
        if (CanMove(data.moveSpeed))
        {
            motor.Move(data.moveSpeed);
        }

        //Else I can't move, avoid the obstacle
        else
        {
            //Rotate until we can move
            avoidanceStage = AvoidanceStage.Rotate;
        }

    }

    //Avoid whatever obstacle is in the way
    private void Avoid()
    {
        switch (avoidanceStage)
        {
            case AvoidanceStage.None:
                break;

            case AvoidanceStage.Rotate:
                //Begin rotating
                motor.Rotate(data.rotateSpeed); //causes the tank to turn to the right each time

                //If can move forward, change avoidance stage to Move
                if (CanMove(data.moveSpeed))
                {
                    //change avoidance stage to Move
                    avoidanceStage = AvoidanceStage.Move;

                    //Set exitTime 
                    exitTime = avoidanceTime; 
                }
                break;

            case AvoidanceStage.Move:
                //If we can move, move until end of timer
                if (CanMove(data.moveSpeed))
                {
                    exitTime -= Time.deltaTime; //deduct time each frame
                    motor.Move(data.moveSpeed); //move the tank forward

                    //If timer has expired
                    if (exitTime <= 0)
                    {
                        //Change avoidance stage to none
                        avoidanceStage = AvoidanceStage.None;
                    }
                }

                //Else we can't move forward
                else
                {
                    //Change to avoidance stage Rotate
                    avoidanceStage = AvoidanceStage.Rotate;
                }

                break;
        }
    }

    public bool CanMove(float speed)
    {
        RaycastHit hit; //create raycast to store our hit data

        //
        if (Physics.Raycast(tf.position, tf.forward, out hit, speed))
        {
            //If our hit is not the player
            if (!hit.collider.CompareTag("Player"))
            {
                //Something is blocking us, return false
                return false;
            }
        }


        return true;
    }


}
