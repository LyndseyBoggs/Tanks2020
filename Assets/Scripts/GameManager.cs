using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject instantiatedPlayerTank; //holds reference to Player tank
    public GameObject playerTankPrefab; //the prefab we want to use to spawn the player
    public GameObject[] enemyTanks; //holds array of references to enemy tanks
    public List<GameObject> playerSpawnPoints;
    public List<GameObject> enemySpawnPoints; //list of enemy spawnpoints

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

    void Update()
    {
        //If the player tank does not exist
        if (instantiatedPlayerTank == null)
        {
            SpawnPlayer(RandomSpawnPoint(playerSpawnPoints));
        }
    }

    private GameObject RandomSpawnPoint(List<GameObject> spawnPoints)
    {
        //Get a random int along the list of spawn point indices
        int spawnToGet = UnityEngine.Random.Range(0, spawnPoints.Count - 1);

        //Return the spawn point from this function
        return spawnPoints[spawnToGet];
    }

    //Spawn the player at a spawn point
    public void SpawnPlayer(GameObject spawnPoint)
    {
        //if we didn't pass a specific spawnpoint, pick a random one
        if (spawnPoint == null)
        {
            //Pick a random spawnpoint
        }

        //Spawn the player at that spawnpoint location
        instantiatedPlayerTank = Instantiate(playerTankPrefab, spawnPoint.transform.position, Quaternion.identity);
    }
}
