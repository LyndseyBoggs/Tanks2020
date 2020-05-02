using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public PowerUp powerup;             //powerup to apply via Pickup
    public AudioClip feedbackAudioClip; //audio to play when picked up
    private Transform tf;               //transform of this game object
    public PickUpSpawner mySpawner;     //used to set isSpawned bool is pickup spawner
    
    // Start is called before the first frame update
    void Start()
    {
        //get my transform
        tf = GetComponent<Transform>();

        //Add self to pickups list in game manager
        GameManager.instance.instantiatedPickUps.Add(this.gameObject);
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
                AudioSource.PlayClipAtPoint(feedbackAudioClip, tf.position, GameManager.instance.fxVolume);
                               
            }

            //set isSpawned in PickupSpawner
            mySpawner.isSpawned = false;

            //Destroy this game object
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        //Remove self from pickups list in game manager
        GameManager.instance.instantiatedPickUps.Remove(this.gameObject);
    }
}
