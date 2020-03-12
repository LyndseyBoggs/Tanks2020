using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]

public class SampleAIControllerTwo : MonoBehaviour
{
    public enum AttackMode { Chase, Flee}       //Attack Modes available to AI
    public AttackMode attackMode;               //Current Attack Mode
    public Transform target;                    //Target to chase or flee from
    public float fleeDistance = 1.0f;           //Distance

    private TankData data;
    private TankMotor motor;

    private Transform tf;
    
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
        //Switch behavior based on current Attack Mode
        switch (attackMode)
        {
            case AttackMode.Chase:
                //Rotate towards the target at rotate speed (note: only rotates to the right! use neg. speed to rotate left)
                motor.RotateTowards(target.position, data.rotateSpeed);

                //move towards target
                motor.Move(data.moveSpeed);

                break;

            case AttackMode.Flee:
                //The vector from enemy to target is the difference of target pos. minus our pos.
                Vector3 vectorToTarget = target.position - tf.position;

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

                break;

            default:
                Debug.Log("Attack mode not implemented");
                break;
        }
    }
}
