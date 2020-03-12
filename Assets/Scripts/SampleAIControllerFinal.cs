using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAIControllerFinal : MonoBehaviour
{
    public enum AIState { Chase, ChaseAndFire, CheckForFlee, Flee, Rest}

    public enum Personalities { Inky, Pinky, Blinky, Clyde}
    public Personalities personality = Personalities.Inky;

    public AIState aiState;

    private TankData data;
    private GameObject player;

    private float stateEnterTime;

    // Start is called before the first frame update
    void Start()
    {
        data = GetComponent<TankData>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (personality)
        {
            case Personalities.Inky:
                break;

            case Personalities.Blinky:
                break;

            case Personalities.Pinky:
                break;

            case Personalities.Clyde:
                break;

            default:
                break;
        }


    }

    private void Clay()
    {
        
    }


    private bool playerIsInRange()
    {        
        return true;
    }

    private void Chase(GameObject target)
    {

    }

    private void Shoot()
    {

    }

    private void Inky()
    {
        switch (aiState)
        {
            case AIState.Chase:
                //Chase(player);
                if (data.health < (data.maxHealth * 0.5))
                {
                    ChangeState(AIState.CheckForFlee);
                }

                else if (playerIsInRange())
                {
                    ChangeState(AIState.ChaseAndFire);
                }
                break;

            case AIState.ChaseAndFire:
                //do state behaviors
                Chase(player);
                Shoot();

                //check for transitions in the order of priority
                if (!playerIsInRange())
                {
                    ChangeState(AIState.Chase);
                }
                break;

            case AIState.CheckForFlee:
                if (playerIsInRange())
                {
                    ChangeState(AIState.Flee);
                }

                else
                {
                    ChangeState(AIState.Rest);
                }
                break;

            case AIState.Flee:
                //Flee from player
                Flee(player);

                //wait 30 seconds then check for flee
                if (stateEnterTime >= stateEnterTime + 30.0f)
                {
                    ChangeState(AIState.CheckForFlee);
                }
                break;

            case AIState.Rest:
                Rest();
                if (playerIsInRange())
                {
                    ChangeState(AIState.Flee);
                }

                //if approximately fully healed
                else if (Mathf.Approximately(data.health, data.maxHealth))
                {
                    ChangeState(AIState.Chase);
                }
                break;

            default:
                break;


        }
    }

    private void Rest()
    {
        //If tank is less than max health
        if (data.health <= data.maxHealth)
        {
            //if regenPts would exceed max health, set health to max
            if (data.health + data.healthRegenPerSec > data.maxHealth)
            {
                data.health = data.maxHealth;
            }

            //Else, heal over time
            else
            {
                //Heal some hitpoints every second
                data.health += data.healthRegenPerSec * Time.deltaTime;
            }
            
        }

        
    }

    private void Flee(GameObject player)
    {
        throw new NotImplementedException();
    }

    private void Blinky()
    {

    }

    private void Pinky()
    {

    }

    private void Clyde()
    {

    }

    public void ChangeState (AIState newState)
    {
        //chase our state 
        aiState = newState;

        //save time we changed states
        stateEnterTime = Time.time;
    }
}
