using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMotor : MonoBehaviour
{
    private CharacterController characterController;
    private TankData data;
    
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
        Move(3);
        Rotate(180);
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
