using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public PowerUp powerup;             //powerup to apply via Pickup
    public AudioClip feedbackAudioClip; //audio to play when picked up
    private Transform tf;               //transform of this game object
    
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //Get the powerup controller on the colliding object
        PowerUpController powCon = other.gameObject.GetComponent<PowerUpController>();

        //Check to see if it has a Powerup Controller
        //It it HAS a powerup controller
        if (powCon != null)
        {
            //Add power up to its controller
            powCon.Add(powerup);

            //Null check for feedback audio
            if (feedbackAudioClip != null)
            {
                //Play feedback sound at location at volume
                //TODO: Adjust volume based on settings
                AudioSource.PlayClipAtPoint(feedbackAudioClip, tf.position, 1.0f);
            }
            
            //Destroy this game object
            Destroy(this.gameObject);
        }
    }
}
