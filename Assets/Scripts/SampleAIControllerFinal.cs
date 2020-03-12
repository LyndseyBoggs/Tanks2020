using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]

public class SampleAIControllerFinal : MonoBehaviour
{
    public enum AIState { Chase, ChaseAndFire, CheckForFlee, Flee, Rest}

    public enum Personalities { Inky, Pinky, Blinky, Clyde}
    public Personalities personality = Personalities.Inky;

    public AIState aiState = AIState.Chase;
    private float stateEnterTime;
    private float healthGainPerSec = 0.5f;

    public float hearingDistance = 25.0f;
    public float FOVAngle = 45.0f;
    public float inSightAngle = 10.0f;

    public enum AvoidanceStage
    { None, Rotate, Move };

    public AvoidanceStage avoidanceStage;
    public float avoidanceTime = 2.0f;

    private float exitTime;

    private TankData data;
    private TankMotor motor;
    private Transform tf;
    public GameObject player;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        //Get and set data and motor on Start
        data = GetComponent<TankData>();
        motor = GetComponent<TankMotor>();
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (personality)
        {
            case Personalities.Inky:
                break;

            case Personalities.Blinky:
                break;

            case Personalities.Pinky:
                break;

            case Personalities.Clyde:
                break;

            default:
                break;
        }


    }

    private bool playerIsInRange()
    {
        //If player is close enough to shoot and we are aiming at player
        return true;
    }

    private void Chase(GameObject target)
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

    private void Shoot()
    {

    }

    private void Inky()
    {
        switch (aiState)
        {
            case AIState.Chase:
                //Chase(player);
                if (data.health < (data.maxHealth * 0.5))
                {
                    ChangeState(AIState.CheckForFlee);
                }

                else if (playerIsInRange())
                {
                    ChangeState(AIState.ChaseAndFire);
                }
                break;

            case AIState.ChaseAndFire:
                //do state behaviors
                Chase(player);
                Shoot();

                //check for transitions in the order of priority
                if (!playerIsInRange())
                {
                    ChangeState(AIState.Chase);
                }
                break;

            case AIState.CheckForFlee:
                if (playerIsInRange())
                {
                    ChangeState(AIState.Flee);
                }

                else
                {
                    ChangeState(AIState.Rest);
                }
                break;

            case AIState.Flee:
                //Flee from player
                Flee(player);

                //wait 30 seconds then check for flee
                if (stateEnterTime >= stateEnterTime + 30.0f)
                {
                    ChangeState(AIState.CheckForFlee);
                }
                break;

            case AIState.Rest:
                Rest();
                if (playerIsInRange())
                {
                    ChangeState(AIState.Flee);
                }

                //if approximately fully healed
                else if (Mathf.Approximately(data.health, data.maxHealth))
                {
                    ChangeState(AIState.Chase);
                }
                break;

            default:
                Debug.Log("Inky AI State not implemented");
                break;


        }
    }

    private void Blinky()
    {
        //TODO: 
        Debug.Log("Blinky is not implemented");
    }

    private void Pinky()
    {
        Debug.Log("Pinky is not implemented");
    }

    private void Clyde()
    {
        Debug.Log("Clyde is not implemented");
    }

    private void Rest()
    {
        //If tank is less than max health
        if (data.health <= data.maxHealth)
        {
            //if regenPts would exceed max health, set health to max
            if (data.health + data.healthRegenPerSec > data.maxHealth)
            {
                data.health = data.maxHealth;
            }

            //Else, heal over time
            else
            {
                //Heal some hitpoints every second
                data.health += data.healthRegenPerSec * Time.deltaTime;
            }

        }
        
    }

    private void Flee(GameObject player)
    {
        throw new NotImplementedException();
    }

    public void ChangeState (AIState newState)
    {
        //chase our state 
        aiState = newState;

        //save time we changed states
        stateEnterTime = Time.time;
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
}
