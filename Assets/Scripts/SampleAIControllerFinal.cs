using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankShooter))]

public class SampleAIControllerFinal : MonoBehaviour
{
    public enum Personalities { Aggro, Swiss, Blind, Coward}         //Tank Personities enum
    public Personalities personality = Personalities.Aggro;          //Designer-chosen personality of tank effects behavior in Update()

    public enum AIState { Chase, ChaseAndFire, CheckForFlee,        //enum of possible AI States
                          Flee, Rest, Patrol }   
    public AIState aiState = AIState.Chase;                         //current AI State of the tank
    private float stateEnterTime;                                   //tracks the time the tank entered its current state

    public enum AvoidanceStage { None, Rotate, Move };              //States of avoidance
    public AvoidanceStage avoidanceStage = AvoidanceStage.None;     //current state of avoidance, default to none
    public float avoidanceTime = 2.0f;                              //The time the tank should move forward to try to avoid and obstacle
    private float exitTime;                                         //Tracks exit time timer in avoidance states

    public Transform[] waypoints;                                   //Designer-friendly Array of waypoint transforms to follow
    public int currentWayPoint = 0;                                 //the current waypoint in the array that the tank is seeking, initialized to array index 0
    public float closeEnough = 0.5f;                                //Designer-friendly The distance at which the tank is "close enough" to the waypoint to progress towards the next waypoint
    public enum LoopType {Stop, Loop, Pingpong }                    //Enum selection for the pattern type of waypoint - patrol the tank should do
    public LoopType loopType;                                       //Designer-chosen loop type
    public bool isPingpongForward = true;                           //tracks if tank is pingponging forward or backward.
    public RoomTerritory territory;                                    //Territory which belongs to this tank to guard, if personality dictates it so

    public float playerRangeDist = 5.0f;                            //the distance in which "Player in Range" should return true
    public float fleeDistance = 8.0f;                               //the distance to which the tank will try to flee from player
    public float hearingDistance = 5.0f;                           //Distance at which tank can hear sounds
    public float hearingMinVolume = 0.5f;                           //The quietest sound the AI can hear
    public float FOVAngle = 45.0f;                                  //Total sight angle
    public float inSightAngle = 10.0f;                              //Smaller angle so tank can "aim" at player w/ this angle
    public float fleeTime = 15.0f;                                  //How long the tank should flee before checking to exit Flee

    private TankData data;          //This tank's TankData
    private TankMotor motor;        //This tank's TankMotor
    private TankShooter shooter;    //This tank's Shooter
    private Transform tf;           //This tank's Transform
    public GameObject player;       //used to manually chase player in current version.
                                    //TODO: to get player from GameManager (not inspector-set)

    private Color sightColor = Color.red;                           //Color to update to draw debug lines, initialized to red
    private Color hearingColor = Color.red;                         //Color for hearing sphere gizmo, init. red.

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

        //Add self to GameManager list of enemies
        GameManager.instance.instantiatedEnemyTanks.Add(this.gameObject);

        //Get and Set Territory from randomly generated room, if not pre-set
        if (!territory)
        {
            //Set Territory to a random territory from the level game object - map generator list of territories
            territory = GameManager.instance.LevelGameObject.GetComponent<MapGenerator>().instantiatedTerritories[UnityEngine.Random.Range(0, GameManager.instance.LevelGameObject.GetComponent<MapGenerator>().instantiatedTerritories.Count)];
        }

        //Get and Set Waypoints array from territory, if waypoints array is null
        if (waypoints == null || waypoints.Length == 0)
        {
            waypoints = territory.waypoints;
        }


    }

    //Debug gizmos 
    private void OnDrawGizmos()
    {
        //Draw hearing radius debug
        Gizmos.color = hearingColor;
        Gizmos.DrawWireSphere(transform.position, hearingDistance);
    }

    // Update is called once per frame
    void Update()
    {
        //Testing
        CanSee(GameManager.instance.instantiatedPlayerTank);
        
        //Draw debug line to player (red if unseen, green if seen)
        Debug.DrawLine(tf.position, GameManager.instance.instantiatedPlayerTank.transform.position, sightColor);

        //If in avoidance state, do avoidance
        if (avoidanceStage != AvoidanceStage.None)
        {
            Avoid();
        }

        //Else --> Run a specific finite state machine in update depending on AI personality
        else
        {
            switch (personality)
            {
                case Personalities.Aggro:
                    Aggro();     //Run Aggro FSM
                    break;

                case Personalities.Blind:
                    Blind();   //Run Blind FSM
                    break;

                case Personalities.Swiss:
                    Swiss();    //Run Swiss FSM
                    break;

                case Personalities.Coward:
                    Coward();    //Run Coward FSM
                    break;

                default:
                    Debug.Log("AI Personality not implemented");
                    break;
            }
        }

        //Handle the shooting cooldown timer
        //If cooldown is greater than 0, count down each frame
        if (data.timeUntilCanShoot > 0)
        {
            data.timeUntilCanShoot -= Time.deltaTime;
        }

        //Else cooldown is complete, Tank can shoot again
        else
        {
            data.canShoot = true;
        }

    }

   private bool playerIsInRange()
    {
        //If player is close enough to shoot and we are aiming at player (get distance)
        //use FOVAngle and inSightAngle

        //If distance to player is less than range distance, return true
        if (Vector3.Distance(tf.position, GameManager.instance.instantiatedPlayerTank.transform.position) <= playerRangeDist)
        {
            return true;
        }

        return false;
    }
    
    //Change to a new AI state
    public void ChangeState(AIState newState)
    {
        //chase our state 
        aiState = newState;

        //save time we changed states
        stateEnterTime = Time.time;
    }

    //Patrol a set of waypoints [HAS OBSTACLE AVOIDANCE]
    public void Patrol()
    {
        //If currently turning to look at waypoint
        if (motor.RotateTowards(waypoints[currentWayPoint].position, data.rotateSpeed))
        {
            //Do Nothing! Do not interrupt the rotating towards waypoint.
        }
        
        //Try to move forward or enter avoidance
        else
        {
            //If I can move, move forward    
            if (CanMove(data.feelDistance))
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

        //If close enough to the waypoint, progress onward. 
        if (Vector3.SqrMagnitude(waypoints[currentWayPoint].position - tf.position) < (closeEnough * closeEnough))  //Note: I think there may be issues with using SqrMagnitude, as the product of two decimals is smaller than its factors. Use numbers greater than 1
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

    //Chase a target object [HAS OBSTACLE AVOIDANCE]
    //TODO: USe a similar function for Aim()
    private void Chase(GameObject target)
    {
        //Face my target
        motor.RotateTowards(target.transform.position, data.rotateSpeed);

        //If I can move, move forward
        if (CanMove(data.feelDistance))
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

    //Flee from a target object [HAS OBSTACLE AVOIDANCE]
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
        
        //If I can move, move forward    
        if (CanMove(data.feelDistance))
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

    //Fire a bullet from the tank [NOW USES COOLDOWN]
    private void Shoot()
    {
        //Call Shoot() from Tank's TankShooter component
        //shooter.Shoot();

        //If the tank is able to shoot
        if (data.canShoot)
        {
            //Shoot a bullet
            shooter.Shoot();

            //Set canShoot to false
            data.canShoot = false;

            //Reset cooldown until tank can shoot again
            data.timeUntilCanShoot = data.fireRate;
        }

        //Note: Shooting cooldown timer is handled in Update()

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

    //Detect whether the tank can move by detecting obstacles in the path ahead
    public bool CanMove(float feelDist)
    {
        RaycastHit hit; //create raycast to store our hit data

        //If the raycast returns an object hit
        if (Physics.Raycast(tf.position, tf.forward, out hit, feelDist))
        {
            //If our hit is not the player or a waypoint
            if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Waypoint"))
            {
                //Something is blocking us, return false
                return false;
            }
        }

        //Nothing is blocking us, return true
        return true;
    }

    public bool CanSee(GameObject target)
    {        
        //Get the position of the target's transform
        Vector3 targetPosition = target.transform.position;

        //Vector to target is target position minus AI position
        Vector3 vectorToTarget = targetPosition - tf.position;

        // Find the angle between the direction our agent is facing (forward in local space) and the vector to the target.
        float angleToTarget = Vector3.Angle(vectorToTarget, tf.forward);

        // if that angle is less than our field of view angle
        if (angleToTarget < FOVAngle)
        {
            // Raycast to store hit data
            RaycastHit hit;            
                    
            //If we hit something with the raycast
            if (Physics.Raycast(tf.position, vectorToTarget, out hit))
            {
                // Check If the first object we hit is our target 
                if (hit.collider.gameObject == target)
                {
                    //set debug color to green
                    sightColor = Color.green;

                    return true;
                }
            }

            
        }
        
        //return false only occurs if we cannot see the target
        //set debug color to red
        sightColor = Color.red;

        return false;             
    }
    
    public bool CanHear(GameObject target)
    {
        // Get the target's NoiseMaker
        NoiseMaker targetNoise = target.GetComponent<NoiseMaker>();

        // If they don't have one, they can't make noise, so return false
        if (!targetNoise)
        {
            hearingColor = Color.red;
            return false;
        }

        // However, If they do have one
        else
        {
            // If volume from target occured in our hearing distance
            if (Vector3.Distance(target.transform.position, tf.position) <= hearingDistance)
            {
                //if volume is loud enough for us to hear (greater than our quietest hearing capabilty)
                if (targetNoise.volume >= hearingMinVolume)
                {
                    hearingColor = Color.green;
                    return true;
                }

                
            }

            // Otherwise, we did not hear it, return false
            hearingColor = Color.red;
            return false;
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
    //Aggro Personality FSM called from Update
    private void Aggro()
    {
        switch (aiState)
        {
            //---CHASE STATE---
            case AIState.Chase:
                //Chase the player
                Chase(GameManager.instance.instantiatedPlayerTank);

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
                Chase(GameManager.instance.instantiatedPlayerTank);
                Shoot();

                //Check for transitions
                //-----------------------------------
                //If player is not in range, switch to Chase
                if (!playerIsInRange())
                {
                    ChangeState(AIState.Chase);
                }

                //If health is less than half capacity
                if (data.health < (data.maxHealth * 0.5))
                {
                    //Check for flee
                    ChangeState(AIState.CheckForFlee);
                }
                break;

            //--CHECK FOR FLEE STATE---
            case AIState.CheckForFlee:
                //If player is still in range --> Flee
                if (playerIsInRange())
                {
                    ChangeState(AIState.Flee);
                }

                //Else player is not in range, so Rest
                else
                {
                    ChangeState(AIState.Rest);
                }
                break;

            //---FLEE SATE---
            case AIState.Flee:
                //Flee from player
                Flee(GameManager.instance.instantiatedPlayerTank);

                //wait "fleeTime" seconds then check for flee
                if (Time.time >= (stateEnterTime + fleeTime))
                {
                    ChangeState(AIState.CheckForFlee);
                }
                break;

            //---REST STATE---
            case AIState.Rest:
                //Rest and regain health
                Rest();

                //Check for transitions
                //-----------------------------------
                //If player comes into range --> Flee
                if (playerIsInRange())
                {
                    ChangeState(AIState.Flee);
                }

                //if approximately fully healed, resume the chase
                else if (Mathf.Approximately(data.health, data.maxHealth))
                {
                    ChangeState(AIState.Chase);
                }
                break;

            //--DEFAULT---
            default:
                Debug.Log("Aggro AI State not implemented");
                break;

        }
    }

    //Blind Personality FSM called from Update
    private void Blind()
    {
        switch (aiState)
        {
            //---CHASE STATE---
            case AIState.Chase:
                //Chase the player
                Chase(GameManager.instance.instantiatedPlayerTank);

                //Check for transitions
                //-----------------------------------
                //If cannot hear player -> rest
                if (!CanHear(GameManager.instance.instantiatedPlayerTank))
                {
                    //rest
                    ChangeState(AIState.Rest);
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
                Chase(GameManager.instance.instantiatedPlayerTank);
                Shoot();

                //Check for transitions
                //-----------------------------------
                //If player is not in range, switch to Chase
                if (!playerIsInRange())
                {
                    ChangeState(AIState.Chase);
                }

                break;

            //---REST STATE---
            case AIState.Rest:
                //Rest and regain health
                Rest();                              

                //Check for transitions
                //-----------------------------------
                //If player is heard, chase the player
                if (CanHear(GameManager.instance.instantiatedPlayerTank))
                {
                    ChangeState(AIState.Chase);
                }

                break;

            //--DEFAULT---
            default:
                Debug.Log("Blind AI State not implemented");
                break;

        }
    }

    //Swiss Personality FSM called from Update
    private void Swiss()
    {
        switch (aiState)
        {
            //---PATROL STATE---
            case AIState.Patrol:
                //Patrol the waypoints
                Patrol();

                //Rest while patroling (regain health0
                Rest();

                //Check for transitions
                //-----------------------------------
                //if shot by player, it's war
                if (data.lastShotBy == GameManager.instance.instantiatedPlayerTank)
                {
                    //Chase the player
                    ChangeState(AIState.Chase);
                }

                break;
            
            //---CHASE STATE---
            case AIState.Chase:
                //Chase the player
                Chase(GameManager.instance.instantiatedPlayerTank);

                //Check for transitions
                //-----------------------------------
                //If player leaves the zone, resume Patrol
                if (territory.isInvaded == false)
                {
                    //reset lastShotBy variable
                    data.lastShotBy = null;
                    
                    //go back to Patrol
                    ChangeState(AIState.Patrol);
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
                Chase(GameManager.instance.instantiatedPlayerTank);
                Shoot();

                //Check for transitions
                //-----------------------------------
                //If player leaves the zone, resume Patrol
                if (territory.isInvaded == false)
                {
                    //reset lastShotBy variable
                    data.lastShotBy = null;

                    //go back to Patrol
                    ChangeState(AIState.Patrol);
                }

                //else if player is not in range, switch to Chase
                else if (!playerIsInRange())
                {
                    ChangeState(AIState.Chase);
                }

                break;

            //--DEFAULT---
            default:
                Debug.Log("Swiss AI State not implemented");
                break;

        }
    }
    
    //Coward Personality FSM called from Update
    private void Coward()
    {
        switch (aiState)
        {                       
            //--CHECK FOR FLEE STATE---
            case AIState.CheckForFlee:
                //Idles in place utnil transition
                
                //If player is still in range --> Flee
                if (playerIsInRange())
                {
                    ChangeState(AIState.Flee);
                }

                break;

            //---FLEE SATE---
            case AIState.Flee:
                //Flee from player
                Flee(GameManager.instance.instantiatedPlayerTank);

                //wait "fleeTime" seconds then check for flee
                if (Time.time >= (stateEnterTime + fleeTime))
                {
                    ChangeState(AIState.CheckForFlee);
                }
                break;

            //--DEFAULT---
            default:
                Debug.Log("Coward AI State not implemented");
                break;

        }
    }
    
}
