using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public PowerUp powerup;
    public AudioClip feedbackAudioClip;
    private Transform tf;
    
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
                AudioSource.PlayClipAtPoint(feedbackAudioClip, tf.position, 1.0f);
            }
            
            //Destroy this game object
            Destroy(this.gameObject);
        }
    }
}
