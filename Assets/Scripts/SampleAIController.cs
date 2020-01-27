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
        if (motor.RotateTowards(waypoints[currentWayPoint].position, data.rotateSpeed))
        {
            //Do Nothing!
        }

        else
        {
            motor.Move(data.moveSpeed);
        }

        //If close enough to the waypoint, progress
        if (Vector3.SqrMagnitude(waypoints[currentWayPoint].position - tf.position) < (closeEnough * closeEnough))
        {
            //if not at the end of the list (avoid index out of range)
            if (currentWayPoint <= waypoints.Length)
            {
                //Target next waypoint in list
                currentWayPoint++;
            }

            //else, we are at end of waypoint list
            else
            {
                //if pingpong

                //if loop
                currentWayPoint = 0; // return to first waypoint
            }
            
            
        }
    }
}
