using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject LevelGameObject;

    public GameObject instantiatedPlayerTank;       //holds reference to Player tank
    public GameObject playerTankPrefab;             //the prefab we want to use to spawn the player
    public List<GameObject> instantiatedEnemyTanks; //list of all enemy tanks currently instantiated
    public List<GameObject> enemyTankPrefabs;       //Designer list of enemy tanks to instantiate
    public List<GameObject> playerSpawnPoints;      //list of player spawn points
    public List<GameObject> enemySpawnPoints;       //list of enemy spawn points

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
            //if the list of player spawn points is not empty
            if (playerSpawnPoints.Count != 0)
            {
                //Spawn the player at a spawn point
                SpawnPlayer(RandomSpawnPoint(playerSpawnPoints));
            }

            else
            {
                Debug.Log("Could not spawn the player at spawn point: Spawn point list is empty.");
            }
            
        }
    }

    public GameObject RandomSpawnPoint(List<GameObject> spawnPoints)
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
        if (playerTankPrefab != null)   //null check for playerTankPrefab
        {
            instantiatedPlayerTank = Instantiate(playerTankPrefab, spawnPoint.transform.position, Quaternion.identity);
        }

        else
        {
            Debug.Log("Player Tank Prefab not assigned in Game Manager");
        }
        
    }

    public void SpawnEnemies()
    {
        //If the list of enemy prefabs is not empty
        if (enemyTankPrefabs.Count != 0)
        {
            for (int i = 0; i < enemyTankPrefabs.Count; i++)
            {
                //Create a new enemy tank in the world from the list
                GameObject instantiatedEnemyTank =
                    Instantiate(enemyTankPrefabs[i], RandomSpawnPoint(enemySpawnPoints).transform.position, Quaternion.identity);

                //Add to list of instantiated enemy tanks
                instantiatedEnemyTanks.Add(instantiatedEnemyTank);
            }
        }

        
    }
}
