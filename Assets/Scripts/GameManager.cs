using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject playerTank; //holds reference to Player tank
    public GameObject[] enemyTanks; //holds array of references to enemy tanks
    public List<Transform> enemySpawnPoints; //list of enemy spawnpoints

    // Runs before any Start() functions run
    void Awake()
    {
        //if no other instance of GameManager exists, store this as the instance
        if (instance == null)
        {
            instance = this;
        }

        //else more than one GameManager is attempting to live, destroy the second one and log error
        else
        {
            Debug.LogError("ERROR: There can only be one GameManager.");
            Destroy(gameObject);
        }

        //Prevents game manager from being destroyed between scenes
        DontDestroyOnLoad(gameObject);
    }
}
