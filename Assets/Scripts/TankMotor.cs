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

//This script requires a TankData component. 
//Create one if none exists
[RequireComponent(typeof(TankData))]

public class TankMotor : MonoBehaviour
{
    private CharacterController characterController; //stores the character controller component from this game object
    private TankData data; // stores the TankData component on this object
    private Transform tf;
    
    // Start is called before the first frame update
    void Start()
    {
        //get the character controller on this object
        characterController = GetComponent<CharacterController>();

        //get the tank data component on this object
        data = GetComponent<TankData>();

        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move(3); //testing Move(), comment out
        //Rotate(180); //testing Rotate(), comment out
    }

    //Move the tank forwards and backwards
    public void Move(float speed)
    {
        // Create a vector to hold our speed data
        Vector3 speedVector = transform.forward * speed;

        characterController.SimpleMove(speedVector);
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



