using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState                           //States the game could possibly be in
    {
        MainMenu, OptionsMenu, StartMenu, Gameplay, Paused, GameOver
    }

    public GameState currentGameState = GameState.MainMenu;    //Tracks the current Game State of the game, initialized to main menu
    public GameState previousGameState;                        //tracks the previous state of the game

    public GameObject LevelGameObject;

    public GameObject instantiatedPlayerTank;       //holds reference to Player tank
    public GameObject playerTankPrefab;             //the prefab we want to use to spawn the player
    public List<GameObject> instantiatedEnemyTanks; //list of all enemy tanks currently instantiated
    public List<GameObject> enemyTankPrefabs;       //Designer list of enemy tanks to instantiate
    public List<GameObject> playerSpawnPoints;      //list of player spawn points
    public List<GameObject> enemySpawnPoints;       //list of enemy spawn points

    public int numberofPlayers;                     //Set to 1 or 2 based on single or multiplayer choice
    

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
                //Debug.Log("Could not spawn the player at spawn point: Spawn point list is empty.");
            }
            
        }
    }

    //Change to a new AI state
    public void ChangeState(GameState newState)
    {
        //save time we changed states
        //stateEnterTime = Time.time; //from ChangeState in SampleAIFinal.cs

        //save current state to previous state
        previousGameState = currentGameState; // set previous game state

        //Change states
        switch (currentGameState)
        {
                

            case GameState.MainMenu:
                //disable input from main menu (better place for this is here, my idea

                if (newState == GameState.OptionsMenu)
                {
                    //Disable input from main menu
                    //Activate options menu
                }

                if (newState == GameState.StartMenu)
                {
                    //Disable input from menu
                    //Activate Game Start Menu
                }

                break;

            case GameState.OptionsMenu:
                if (newState == GameState.MainMenu)
                {
                    //Save changes to options
                    //Deactivate options menu
                    //reactivate main menu
                }

                if (newState == GameState.Paused)
                {
                    //save changes to options
                    //Deactivate options menu
                    //Activate pause menu
                }

                break;

            case GameState.StartMenu:
                //
                if (newState == GameState.MainMenu)
                {
                    //Deactivate Start Menu
                    //Activate Main Menu
                }

                if (newState == GameState.Gameplay)
                {
                    //Deactiviate start menu
                    //Load level, spawn players and enemies
                    LevelGameObject.GetComponent<MapGenerator>().StartGame();
                }

                break;

            case GameState.Gameplay:
                if (newState == GameState.Paused)
                {
                    //Pause the simulation
                    //Pull up pause menu
                }

                if (newState == GameState.GameOver)
                {
                    //handle game over behaviors
                    //save high score
                    //stop simulation (if desired)
                    //Restart Game button
                }


                break;

            case GameState.Paused:
                if (newState == GameState.Gameplay)
                {
                    //resume simulation
                    //remove pause menu
                }

                if (newState == GameState.MainMenu)
                {
                    //Switch to main menu scene & end simulation
                }

                if (newState == GameState.OptionsMenu)
                {
                    //Deactivate pause menu UI
                    //Activate options menu UI
                }

                break;

            case GameState.GameOver:
                if (newState == GameState.Gameplay)
                {
                    //Restart the game (end simulation & restart)
                }

                if (newState == GameState.MainMenu)
                {
                    //Switch to main menu scene & end simulation
                }

                break;

            default:
                Debug.Log("GameState not implemented");
                break;
        }

        //change our state to the new state
        currentGameState = newState;

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
        if (enemyTankPrefabs.Count > 0)
        {
            for (int i = 0; i < enemyTankPrefabs.Count; i++)
            {
                //Create a new enemy tank in the world from the list
                GameObject instantiatedEnemyTank =
                    Instantiate(enemyTankPrefabs[i], RandomSpawnPoint(enemySpawnPoints).transform.position, Quaternion.identity);

                //Note: Enemies add themelves to list of instantiated enemy tanks on Start() of AI Controller                
            }
        }

        
    }
}
