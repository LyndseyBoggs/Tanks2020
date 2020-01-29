using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]

public class SampleAIController : MonoBehaviour
{
    public Transform[] waypoints;
    private TankData data;
    private TankMotor motor;
    private Transform tf; 
    public int currentWayPoint = 0;
    public float closeEnough; 

    public enum LoopType
    {
        Stop,
        Loop,
        Pingpong
    }

    public LoopType loopType;
    public bool isPingpongForward = true; //tracks if tank is pingponging forward or backward.

    // Start is called before the first frame update
    void Start()
    {
        data = GetComponent<TankData>();
        motor = GetComponent<TankMotor>();
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //If not currently turning to look at waypoint
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
}
