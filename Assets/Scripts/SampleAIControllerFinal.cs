using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]

public class SampleAIControllerFinal : MonoBehaviour
{
    public enum Personalities { Inky, Pinky, Blinky, Clyde}
    public Personalities personality = Personalities.Inky;

    public enum AIState { Chase, ChaseAndFire, CheckForFlee, Flee, Rest }
    public AIState aiState = AIState.Chase;
    private float stateEnterTime;
    
    public float hearingDistance = 25.0f;
    public float FOVAngle = 45.0f;
    public float inSightAngle = 10.0f;
    public float fleeTime = 15.0f;      //How long the tank should flee before checking to exit Flee

    public enum AvoidanceStage
    { None, Rotate, Move };

    public AvoidanceStage avoidanceStage;
    public float avoidanceTime = 2.0f;

    private float exitTime;

    private TankData data;
    private TankMotor motor;
    private Transform tf;
    public GameObject player;       //used to manually chase player in current version.
                                    //Will need to update to get player from GameManager

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
        //Run a specific finite state machine in update depending on AI personality
        switch (personality)
        {
            case Personalities.Inky:
                Inky();     //Run Inky FSM
                break;

            case Personalities.Blinky:
                Blinky();   //Run Blinky FSM
                break;

            case Personalities.Pinky:
                Pinky();    //Run Pinky FSM
                break;

            case Personalities.Clyde:
                Clyde();    //Run Clyde FSM
                break;

            default:
                Debug.Log("AI Personality not implemented");
                break;
        }


    }

    //TODO: Implement Player is In Range
    private bool playerIsInRange()
    {
        //If player is close enough to shoot and we are aiming at player (get distance)
        return true;
    }

    //Detect whether the tank can move by detecting obstacles in the path ahead
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

        //Nothing is blocking us, return true
        return true;
    }

    //Change to a new AI state
    public void ChangeState(AIState newState)
    {
        //chase our state 
        aiState = newState;

        //save time we changed states
        stateEnterTime = Time.time;
    }

    //Chase a target object
    private void Chase(GameObject target)
    {
        //Face my target
        motor.RotateTowards(target.transform.position, data.rotateSpeed);

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

    //Flee from a target object
    //TODO: Implement Flee
    private void Flee(GameObject fleeFromObject)
    {
        throw new NotImplementedException();
    }

    //Fire a bullet from the tank
    //TODO: Implement Shoot
    private void Shoot()
    {

    }

    //Heal tank over time
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

    //Avoid whatever obstacle is in the way using Avoidance FSM
    private void Avoid()
    {
        //FSM of avoidance states
        switch (avoidanceStage)
        {
            //---NO AVOIDANCE STATE---
            case AvoidanceStage.None:
                //Do nothing
                break;

            //---ROTATE TO AVOID STATE---
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

            //---MOVE TO AVOID STATE---
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

    //-------------------------------------------------------
    //  Personality FMS
    //-------------------------------------------------------
    //Inky Personality FSM called from Update
    private void Inky()
    {
        switch (aiState)
        {
            //---CHASE STATE---
            case AIState.Chase:
                //Chase the player
                Chase(player);

                //Check for transitions
                //-----------------------------------
                //If health is less than half capacity
                if (data.health < (data.maxHealth * 0.5))
                {
                    //Check for flee
                    ChangeState(AIState.CheckForFlee);
                }

                //Else if player is in range
                else if (playerIsInRange())
                {
                    //Chase and fire at the player
                    ChangeState(AIState.ChaseAndFire);
                }
                break;

            //--CHASE AND FIRE STATE---
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

            //--CHECK FOR FLEE STATE---
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

            //---FLEE SATE---
            case AIState.Flee:
                //Flee from player
                Flee(player);

                //wait 30 seconds then check for flee
                if (Time.time >= (stateEnterTime + fleeTime))
                {
                    ChangeState(AIState.CheckForFlee);
                }
                break;

            //---REST STATE---
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

            //--DEFAULT---
            default:
                Debug.Log("Inky AI State not implemented");
                break;

        }
    }

    //Blinky Personality FSM called from Update
    //TODO: Blinky FSM
    private void Blinky()
    {
        //TODO: 
        Debug.Log("Blinky is not implemented");
    }

    //Pinky Personality FSM called from Update
    //TODO: Pinky FSM
    private void Pinky()
    {
        Debug.Log("Pinky is not implemented");
    }

    //TODO: Clyde FSM
    //Clyde Personality FSM called from Update
    private void Clyde()
    {
        Debug.Log("Clyde is not implemented");
    }
    
}
