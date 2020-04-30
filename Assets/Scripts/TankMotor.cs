/*
 *  Programmer: Lyndsey Boggs
 *  Date: January 2020
 *  Summary: This script controls the movement of the tank, which are called from the Input Controller
 */

using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using ADBannerView = UnityEngine.iOS.ADBannerView;
using Vector3 = UnityEngine.Vector3;

//This script requires a TankData component and a Noisemaker
//Create one if none exists
[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(NoiseMaker))]

public class TankMotor : MonoBehaviour
{
    private CharacterController characterController; //stores the character controller component from this game object
    private TankData data; // stores the TankData component on this object
    private Transform tf;
    private NoiseMaker noiseMaker;
    
    // Start is called before the first frame update
    void Start()
    {
        //get the character controller on this object
        characterController = GetComponent<CharacterController>();

        //get the tank data component on this object
        data = GetComponent<TankData>();

        tf = GetComponent<Transform>();

        noiseMaker = GetComponent<NoiseMaker>();

        if (!noiseMaker)
        {
            noiseMaker = gameObject.AddComponent<NoiseMaker>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Move the tank forwards and backwards
    public void Move(float speed)
    {
        // Create a vector to hold our speed data
        Vector3 speedVector = transform.forward * speed;

        characterController.SimpleMove(speedVector);

        //Affect sound while moving or not moving
        //Tank is moving
        if (speed != 0)
        {
            //Tank is moving, set volume to move volume
            noiseMaker.volume = data.moveVolume;
        }

        //else, tank is not moving
        else
        {
            //Tank has stopped moving (a speed of 0.0f is received each frame for SimpleMove from InputController)
            //if volume has not reached 0, decrease volume each frame
            if (noiseMaker.volume > 0)
            {
                //subtract 1 unit of volume each second
                noiseMaker.volume -= Time.deltaTime;
            }

            //if volume falls below 0, reset to 0 (neg. volume is illogical)
            if (noiseMaker.volume < 0)
            {
                noiseMaker.volume = 0;
            }
        }

    }

    //Rotates the tank left and right
    //Positive speed rotates right, negative speed rotates left
    public void Rotate(float speed)
    {
        // Create a vector to hold our rotation data
        Vector3 rotateVector = Vector3.up * speed * Time.deltaTime;

        // Now, rotate our tank by this value - we want to rotate in our local space (not in world space).
        transform.Rotate(rotateVector, Space.Self);
    }


    public bool RotateTowards(Vector3 target, float speed)
    {
        Vector3 vectorToTarget = target - tf.position;

        // Find the Quaternion that looks down that vector
        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);

        // If that is the direction we are already looking, we don't need to turn!
        if (targetRotation == tf.rotation)
        {
            return false;
        }

        // Otherwise:
        // Change our rotation so that we are closer to our target rotation, but never turn faster than our Turn Speed
        //   Note that we use Time.deltaTime because we want to turn in "Degrees per Second" not "Degrees per Framedraw"
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, speed * Time.deltaTime);

        // We rotated, so return true
        return true;
    }

}



