using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float damage;
    public float lifeTime = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Destroy cannonball after lifetime
        lifeTime -= Time.deltaTime; //subtract the time passed each frame

        //when life is over, destroy object
        if (lifeTime <= 0) 
        {
            Destroy(this.gameObject);
        }
    }

    //Destroy Cannonball on collision with any object
    void OnCollisionEnter(Collision collision)
    {
        //If collides with anything, destroy
        Destroy(this.gameObject);
    }


}
