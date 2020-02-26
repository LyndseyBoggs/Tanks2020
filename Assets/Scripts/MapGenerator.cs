using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum MapType {MapOfTheDay, Random, Seeded}   //Type of map to generate

    public MapType mapType = MapType.Random; //public variable to store user selection, defaulted to Random

    public int mapSeed;     //Designer-friendly variable to input specific map seed

    public int MapRows = 3;
    public int MapCols = 3;

    private float roomWidth = 50;
    private float roomLength = 50;

    public GameObject[] roomPrefabs;

    private Room[,] grid; //Two-dimensional array for rooms to act as our grid, may be moved to Game Manager

    // Start is called before the first frame update
    void Start()
    {
        switch (mapType)
        {
            case MapType.MapOfTheDay:
                mapSeed = DateToInt(DateTime.Now.Date); //use the current date 
                break;

            case MapType.Random:
                mapSeed = DateToInt(DateTime.Now); //use the current exact time (now)
                break;

            case MapType.Seeded:
                //Do nothing, mapSeed is already set to user input
                break;

            default:
                Debug.Log("[MapGenerator] Map type not implemented");
                break;
        }
        
        //Generate the grid on start
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Return a room prefab game object at random
    public GameObject GetRandomRoom()
    {
        return roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Length)];
    }

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


    //Randomly generate the grid for the level
    public void GenerateGrid()
    {
        UnityEngine.Random.seed = mapSeed;

        grid = new Room[MapCols,MapRows];
        
        //for each row
        for (int row = 0; row < MapRows; row++)
        {
            //for each column in row
            for (int col = 0; col < MapCols; col++)
            {
                float xPosition = roomWidth * col;
                float zPosition = roomLength * row;

                Vector3 newPosition = new Vector3(xPosition, 0.0f, zPosition);

                //Instantiate random room at position at prefab rotation
                GameObject tempRoomObj = Instantiate(GetRandomRoom(), newPosition, Quaternion.identity) as GameObject;

                //Set our room's parent object
                tempRoomObj.transform.parent = this.transform;

                //Rename our room object as Room_x,y
                tempRoomObj.name = "Room_" + col + "," + row;

                //Store room in 2d array of grid
                Room tempRoom = tempRoomObj.GetComponent<Room>();
                grid[col, row] = tempRoom;

                //If room is at bottom of grid, open North door
                if (row == 0)
                {
                    tempRoom.doorNorth.SetActive(false);
                }

                //If room is at top of grid, open the south door
                else if (row == MapRows - 1)
                {
                    tempRoom.doorSouth.SetActive(false);
                }

                //Else is in the middle, open South and North Doors
                else
                {
                    tempRoom.doorSouth.SetActive(false);
                    tempRoom.doorNorth.SetActive(false);
                }

                if (col == 0)
                {
                    tempRoom.doorEast.SetActive(false);
                }

                else if (col == MapCols - 1)
                {
                    tempRoom.doorWest.SetActive(false);
                }

                else
                {
                    tempRoom.doorEast.SetActive(false);
                    tempRoom.doorWest.SetActive(false);
                }
            }
        }
    }
}
