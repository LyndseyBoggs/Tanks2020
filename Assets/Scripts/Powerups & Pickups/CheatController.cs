using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PowerUpController))]

public class CheatController : MonoBehaviour
{
    private PowerUpController powCon;
    public PowerUp CheatPowerUp;
    
    // Start is called before the first frame update
    void Start()
    {
        if (powCon == null)
        {
            powCon = GetComponent<PowerUpController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Add cheat power up on F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            powCon.Add(CheatPowerUp);
        }
    }
}
