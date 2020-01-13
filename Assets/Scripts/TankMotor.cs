/*
 *  Programmer: Lyndsey Boggs
 *  Date: January 2020
 *  Summary: This script controls the movement of the tank, which are called from the Input Controller
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires a TankData component. 
//Create one if none exists
[RequireComponent(typeof(TankData))]

public class TankMotor : MonoBehaviour
{
    private CharacterController characterController; //stores the character controller component from this game object
    private TankData data; // stores the TankData component on this object
    
    // Start is called before the first frame update
    void Start()
    {
        //get the character controller on this object
        characterController = GetComponent<CharacterController>();

        //get the tank data component on this object
        data = GetComponent<TankData>();
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

        // Start with the vector pointing the same direction as the GameObject this script is on.
        //speedVector = transform.forward;

        // Now, instead of our vector being 1 unit in length, apply our speed value
        //speedVector *= speed;

        // Call SimpleMove() and send it our vector
        // Note that SimpleMove() will apply Time.deltaTime, and convert to meters per second for us!
        characterController.SimpleMove(speedVector);
    }

    //Rotates the tank left and right
    //Positive speed rotates right, negative speed rotates left
    public void Rotate(float speed)
    {
        // Create a vector to hold our rotation data
        Vector3 rotateVector = Vector3.up * speed * Time.deltaTime;

        // Start by rotating right by one degree per frame draw. Left is just "negative right".
        //rotateVector = Vector3.up;

        // Adjust our rotation to be based on our speed
        //rotateVector *= speed;

        // Transform.Rotate() doesn't account for speed, so we need to change our rotation to "per second" instead of "per frame."
        // See the lecture on Timers for details on how this works.
        //rotateVector *= Time.deltaTime;

        // Now, rotate our tank by this value - we want to rotate in our local space (not in world space).
        transform.Rotate(rotateVector, Space.Self);
    }
}
