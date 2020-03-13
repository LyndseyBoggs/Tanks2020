using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankShooter))]

public class SampleAIControllerFinal : MonoBehaviour
{
    public enum Personalities { Inky, Pinky, Blinky, Clyde}
    public Personalities personality = Personalities.Inky;

    public enum AIState { Chase, ChaseAndFire, CheckForFlee, Flee, Rest, Patrol }
    public AIState aiState = AIState.Chase;
    private float stateEnterTime;

    public Transform[] waypoints;
    public int currentWayPoint = 0;
    public float closeEnough;
    public enum LoopType {Stop, Loop, Pingpong }
    public LoopType loopType;
    public bool isPingpongForward = true; //tracks if tank is pingponging forward or backward.

    public float playerRangeDist = 5.0f;    //the distance in which "Player in Range" should return true
    public float fleeDistance = 8.0f;       //the distance to which the tank will try to flee from player
    public float hearingDistance = 25.0f;   //Distance at which tank can hear sounds
    public float FOVAngle = 45.0f;          //Total sight angle
    public float inSightAngle = 10.0f;      //Smaller angle so tank can "aim" at player w/ this angle
    public float fleeTime = 15.0f;          //How long the tank should flee before checking to exit Flee

    public enum AvoidanceStage { None, Rotate, Move };              //States of avoidance
    public AvoidanceStage avoidanceStage = AvoidanceStage.None;     //current state of avoidance, default to none
    public float avoidanceTime = 2.0f;                              //The time the tank should move forward to try to avoid and obstacle
    private float exitTime;                                         //Tracks exit time timer in avoidance states

    private TankData data;          //This tank's TankData
    private TankMotor motor;        //This tank's TankMotor
    private TankShooter shooter;    //This tank's Shooter
    private Transform tf;           //This tank's Transform
    public GameObject player;       //used to manually chase player in current version.
                                    //Will need to update to get player from GameManager

    // Start is called before the first frame update
    void Start()
    {
        //Get and set data and motor on Start
        data = GetComponent<TankData>();
        motor = GetComponent<TankMotor>();
        tf = GetComponent<Transform>();
        shooter = GetComponent<TankShooter>();

        //get reference to manager player (temporary until next milestone)
        player = GameManager.instance.instantiatedPlayerTank;

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
        //use FOVAngle and inSightAngle
        return true;
    }

    //Detect whether the tank can move by detecting obstacles in the path ahead
    public bool CanMove(float speed)
    {
        RaycastHit hit; //create raycast to store our hit data

        //If the raycast returns an object hit
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

    //Patrol a set of waypoints
    public void Patrol()
    {
        //If currently turning to look at waypoint
        if (motor.RotateTowards(waypoints[currentWayPoint].position, data.rotateSpeed))
        {
            //Do Nothing!
        }

        //Move forward
        else
        {
            motor.Move(data.moveSpeed);
        }

        //If close enough to the waypoint, progress
        if (Vector3.SqrMagnitude(waypoints[currentWayPoint].position - tf.position) < (closeEnough * closeEnough))
        {
            switch (loopType)
            {
                case LoopType.Stop:
                    //if next waypoint will not exceed the array (avoid index out of range)
                    if (currentWayPoint + 1 < waypoints.Length)
                    {
                        //Target next waypoint
                        currentWayPoint++;
                    }
                    break;

                case LoopType.Loop:
                    //if next waypoint will not exceed the array (avoid index out of range)
                    if (currentWayPoint + 1 < waypoints.Length)
                    {
                        //Target next waypoint
                        currentWayPoint++;
                    }

                    //else, we are at end of waypoint list
                    else
                    {
                        //if pingpong

                        //if loop
                        currentWayPoint = 0; // return to first waypoint
                    }
                    break;

                case LoopType.Pingpong:

                    //PingPong Forward
                    if (isPingpongForward == true)
                    {
                        //if next waypoint will not exceed the array (avoid index out of range)
                        if (currentWayPoint + 1 < waypoints.Length)
                        {
                            //Target next waypoint
                            currentWayPoint++;
                        }

                        //else, we are at end of waypoint list -> pingpong backwards now
                        else
                        {
                            isPingpongForward = false;

                            //if loop
                            currentWayPoint--; // return to first waypoint
                        }
                    }

                    //PingPong Backwards
                    else
                    {
                        //if next waypoint will not exceed the array beginning(avoid index out of range)
                        if (currentWayPoint - 1 >= 0)
                        {
                            //Target next reverse waypoint
                            currentWayPoint--;
                        }

                        //else, we are at start of waypoint list -> pingpong forwards now
                        else
                        {
                            isPingpongForward = true;

                            //if loop
                            currentWayPoint++; // return to first waypoint
                        }
                    }



                    break;

                default:
                    Debug.Log("Loop type not implemented.");
                    break;
            }




        }
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
    private void Flee(GameObject target)
    {
        //The vector from enemy to target is the difference of target pos. minus our pos.
        Vector3 vectorToTarget = target.transform.position - tf.position;

        //Get vector away from target (Flip by -1)
        Vector3 vectorAwayFromTarget = -1 * vectorToTarget;

        //Normalize away vector (sets to a magnitude of 1)
        vectorAwayFromTarget.Normalize();

        //Multiply direction to run (vectorAway) by how far away to run (flee distance)
        vectorAwayFromTarget *= fleeDistance;

        //Locate flee location point in space relative to our position to seek
        Vector3 fleePosition = vectorAwayFromTarget + tf.position;

        //Rotate towards the flee position
        motor.RotateTowards(fleePosition, data.rotateSpeed);

        //move forward
        motor.Move(data.moveSpeed);
    }

    //Fire a bullet from the tank
    private void Shoot()
    {
        //Call Shoot() from Tank's TankShooter component
        shooter.Shoot();
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
                //TODO: Integrate obstacle avoidance

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

                //wait "fleeTime" seconds then check for flee
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
