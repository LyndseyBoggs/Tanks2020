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
    public List<GameObject> instantiatedPickUps;    //list of all pickups currently instantiated
    public List<GameObject> enemyTankPrefabs;       //Designer list of enemy tanks to instantiate
    public List<GameObject> playerSpawnPoints;      //list of player spawn points
    public List<GameObject> enemySpawnPoints;       //list of enemy spawn points

    public int highScore;           //stores highest score ever achieved in game
    public List<ScoreData> highScores;

    public float fxVolume;          //
    public float musicVolume;       //

    public int numberofPlayers;     //Set to 1 or 2 based on single or multiplayer choice
    

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

    private void Start()
    {
        highScores = new List<ScoreData>(); //initialize high scores list
        LoadPrefs();
        highScores.Sort(); //sorts scores lowest to highest
        highScores.Reverse(); //flips to highest to lowest
        highScores = highScores.GetRange(0, 5); //get top scores only
    }

    void Update()
    {        
        //If the player tank does not exist (it has been destroyed)
        if (instantiatedPlayerTank == null)
        {
            //TODO: If player lives > 0

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
                        //Chase time scale to 0, but test it out first (not good with AddForce)
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
                    //Restart the game (end simulation & restart) (reload the gameplay scene)
                }

                if (newState == GameState.MainMenu)
                {
                    //Switch to main menu scene & end simulation
                    //Activate the main menu
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
        //seed the Random with the exact time
        UnityEngine.Random.seed = DateToInt(DateTime.Now);

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

    public void LoadPrefs()
    {
        //If high score has previously been saved
        if (PlayerPrefs.HasKey("HighScore"))
        {
            //load high score into game
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        //else, no high score was found to load
        else
        {
            //set high score to 0 (there is no high score yet)
            highScore = 0;
        }

        //If music volume has previously been saved
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            //load previous music volume into music volume
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        //else, no music volume was saved
        else
        {
            //Set to default of max volume
            musicVolume = 1.0f;
        }

        //If FX Volume has previously been saved
        if (PlayerPrefs.HasKey("FXVolume"))
        {
            //load previous fx volume into music volume
            fxVolume = PlayerPrefs.GetFloat("FXVolume");
        }
        //else, no fx volume was saved
        else
        {
            //Set to default of max volume
            fxVolume = 1.0f;
        }

        if (PlayerPrefs.HasKey("Score1"))
        {
            highScores[0].name = PlayerPrefs.GetString("Name1");
            highScores[0].score = PlayerPrefs.GetFloat("Score1");
        }

        else
        {
            highScores.Add(new ScoreData("LMB", 0));
        }


    }

    public void SavePrefs()
    {
        //Set PlayerPrefs variables and save prefs
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("FXVolume", fxVolume);

        //Save names
        PlayerPrefs.SetString("Name1", highScores[0].name);
        PlayerPrefs.SetString("Name2", highScores[1].name);
        PlayerPrefs.SetString("Name3", highScores[2].name);
        PlayerPrefs.SetString("Name4", highScores[3].name);
        PlayerPrefs.SetString("Name5", highScores[4].name);
        PlayerPrefs.SetString("Name6", highScores[5].name); //might be extra, included just in case

        //Save Scores
        PlayerPrefs.SetFloat("Score1", highScores[0].score);
        PlayerPrefs.SetFloat("Score2", highScores[1].score);
        PlayerPrefs.SetFloat("Score3", highScores[2].score);
        PlayerPrefs.SetFloat("Score4", highScores[3].score);
        PlayerPrefs.SetFloat("Score5", highScores[4].score);
        PlayerPrefs.SetFloat("Score6", highScores[5].score); //might be extra, included just in case

        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SavePrefs();
    }

    //Get date as integer
    public int DateToInt(DateTime dateToUse)
    {
        //Return the exact time (down to the millisecond) added together
        return dateToUse.Year +
               dateToUse.Month +
               dateToUse.Day +
               dateToUse.Hour +
               dateToUse.Minute +
               dateToUse.Millisecond;
    }
}
