using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float damage;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyForce(Vector3 force)
    {
        rb.AddForce(force);
    }
}
