using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    //Objects in Scene
    public GameObject LevelGameObject;              //Map Generator Object
    public GameObject instantiatedPlayerTank01;       //holds reference to Player 01 tank
    public GameObject instantiatedPlayerTank02;       //holds reference to Player 02 tank
    public GameObject playerTankPrefab;             //the prefab we want to use to spawn the player
    public List<GameObject> instantiatedEnemyTanks; //list of all enemy tanks currently instantiated
    public List<GameObject> instantiatedPickUps;    //list of all pickups currently instantiated
    public List<GameObject> enemyTankPrefabs;       //Designer list of enemy tanks to instantiate
    public List<GameObject> playerSpawnPoints;      //list of player spawn points
    public List<GameObject> enemySpawnPoints;       //list of enemy spawn points

    //Scores,  Prefs and Settings
    public int highScore;               //stores highest score ever achieved in game
    public List<ScoreData> highScores;
    public float fxVolume = 1.0f;              //
    public float musicVolume = 1.0f;           //    

    //Player variables
    public int numberofPlayers;         //Set to 1 or 2 based on single or multiplayer choice
    public float player01Score;
    public float player02Score;
    public int playerLivesStart = 3;
    public int player01Lives;
    public int player02Lives;
    public bool isGameOver = false;

    //UI Menues
    public GameObject MainMenuObject;
    public GameObject OptionsMenuObject;
    public GameObject StartMenuObject;
    public GameObject PausedMenuObject;
    public GameObject GameOverMenuObject;
    public UIManager UIManager;
    

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

    void Start()
    {
        //Start with full lives
        player01Lives = playerLivesStart;
        player02Lives = playerLivesStart;
        

        highScores = new List<ScoreData>(); //initialize high scores list
        LoadPrefs();

        if (highScores.Count > 0)
        {
            highScores.Sort(); //sorts scores lowest to highest
            highScores.Reverse(); //flips to highest to lowest
            highScores = highScores.GetRange(0, 5); //get top scores only 
        }
               
    }

    void Update()
    {
        //cheat start
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LevelGameObject.GetComponent<MapGenerator>().StartGame();
        }
        
        //If the player 01 tank does not exist (it has been destroyed)
        if (instantiatedPlayerTank01 == null)
        {     
            //if the list of player spawn points is not empty
            if (playerSpawnPoints.Count != 0)
            {
                //If player 1 has lives remaining
                if (player01Lives > 0)
                {
                    //Spawn the player at a spawn point
                    SpawnPlayer(RandomSpawnPoint(playerSpawnPoints), 1);
                }
                
            }
            else
            {
                //Debug.Log("Could not spawn player 1 at spawn point: Spawn point list is empty.");
            }            
        }
        
        //If in Two-Player mode
        if (numberofPlayers == 2)
        {
            //If the player 02 tank does not exist (it has been destroyed)
            if (instantiatedPlayerTank02 == null)
            {
                //if the list of player spawn points is not empty
                if (playerSpawnPoints.Count != 0)
                {
                    //If player 1 has lives remaining
                    if (player02Lives > 0)
                    {
                        //Spawn the player at a spawn point
                        SpawnPlayer(RandomSpawnPoint(playerSpawnPoints), 2);
                    }

                }
                else
                {
                    //Debug.Log("Could not spawn player 1 at spawn point: Spawn point list is empty.");
                }
            }
        }

        //Trigger game over
        //If the game is running and player 1 has died
        if (!isGameOver && player01Lives <= 0)
        {
            //And if in multiplayer mode
            if (numberofPlayers == 2)
            {
                //And if player 2 has died
                if (player02Lives <= 0)
                {
                    isGameOver = true;
                    ChangeState(GameState.GameOver);
                }
            }

            //else in single player mode
            else
            {
                isGameOver = true;
                ChangeState(GameState.GameOver);
            }
            
        }
        
    }
       

    //Change to a new AI state
    public void ChangeState(GameState newState)
    {        
        //save current state to previous state
        previousGameState = currentGameState; // set previous game state

        //Change states        
        switch (currentGameState)
        {               

            case GameState.MainMenu:
                //Turn off main menu
                MainMenuObject.SetActive(false);

                if (newState == GameState.OptionsMenu)
                {
                    //Activate options menu
                    OptionsMenuObject.SetActive(true);
                }

                if (newState == GameState.StartMenu)
                {
                    //Activate Game Start Menu
                    StartMenuObject.SetActive(true);
                }

                break;

            case GameState.OptionsMenu:
                //TODO: Save changes to options

                //Deactivate options menu
                OptionsMenuObject.SetActive(false);

                if (newState == GameState.MainMenu)
                {
                    //reactivate main menu
                    MainMenuObject.SetActive(true);
                }

                if (newState == GameState.Paused)
                {
                    //Activate pause menu
                    PausedMenuObject.SetActive(true);
                }

                break;

            case GameState.StartMenu:
                //Deactivate Start Menu
                StartMenuObject.SetActive(false);

                if (newState == GameState.MainMenu)
                {
                    //Activate Main Menu
                    MainMenuObject.SetActive(true);
                }

                if (newState == GameState.Gameplay)
                {
                    //Set selections from Start menu
                    switch (UIManager.mapTypeDrop.options[UIManager.mapTypeDrop.value].text)
                    {
                        case "Map of the Day":
                            LevelGameObject.GetComponent<MapGenerator>().mapType = MapGenerator.MapType.MapOfTheDay;
                            break;
                        case "Seeded":
                            LevelGameObject.GetComponent<MapGenerator>().mapType = MapGenerator.MapType.Seeded;                            
                            LevelGameObject.GetComponent<MapGenerator>().mapSeed = int.Parse(UIManager.mapSeedInput.text);                            

                            break;
                        case "Random":
                            LevelGameObject.GetComponent<MapGenerator>().mapType = MapGenerator.MapType.Random;
                            break;
                        default:
                            Debug.Log("Spelling error in Map drop down caused default in GameState.StartMenu");
                            break;
                    }


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
                    GameOverMenuObject.SetActive(true);
                }


                break;

            case GameState.Paused:
                //Deactivate pause menu UI
                PausedMenuObject.SetActive(false);

                if (newState == GameState.Gameplay)
                {
                    //resume simulation                    
                }

                if (newState == GameState.MainMenu)
                {
                    //Switch to main menu scene & end simulation
                }

                if (newState == GameState.OptionsMenu)
                {
                    //Activate options menu UI
                    OptionsMenuObject.SetActive(true);
                }

                break;

            case GameState.GameOver:
                //Deactivate Game Over
                GameOverMenuObject.SetActive(false);

                if (newState == GameState.Gameplay)
                {
                    //Restart the game (end simulation & restart) (reload the gameplay scene)
                    SceneManager.LoadScene("MainGame");
                }

                if (newState == GameState.MainMenu)
                {
                    //Switch to main menu scene & end simulation
                    //Activate the main menu
                    MainMenuObject.SetActive(true);
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
    public void SpawnPlayer(GameObject spawnPoint, int playerNumber)
    {
        //if we didn't pass a specific spawnpoint, pick a random one
        if (spawnPoint == null)
        {
            //Pick a random spawnpoint
        }

        //Spawn the player at that spawnpoint location
        if (playerTankPrefab != null)   //null check for playerTankPrefab
        {
            //If player 1 is passed in
            if (playerNumber == 1)
            {
                //Spawn player 1 at spawn point
                instantiatedPlayerTank01 = Instantiate(playerTankPrefab, spawnPoint.transform.position, Quaternion.identity);
                
                instantiatedPlayerTank01.GetComponent<TankData>().playerNumber = 1;                                //set player number to 1
                instantiatedPlayerTank01.gameObject.name = "Player 01";                                            //name to "Player 01"  
                instantiatedPlayerTank01.GetComponent<InputController>().input = InputController.InputScheme.WASD; //Set input scheme
            }

            //Else if player 2 is passed in
            else if (playerNumber == 2)
            {
                //Spawn player 2 at spawn point
                instantiatedPlayerTank02 = Instantiate(playerTankPrefab, spawnPoint.transform.position, Quaternion.identity);

                instantiatedPlayerTank02.GetComponent<TankData>().playerNumber = 2;                                     //set player number to 2
                instantiatedPlayerTank02.gameObject.name = "Player 02";                                                 //name to "Player 02"  
                instantiatedPlayerTank02.GetComponent<InputController>().input = InputController.InputScheme.arrowKeys; //Set input scheme
            }

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
