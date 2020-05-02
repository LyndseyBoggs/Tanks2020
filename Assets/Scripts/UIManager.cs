﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Dropdown mapTypeDrop;    //dropdown for user-selected maptype
    public InputField mapSeedInput;      //input field for map seed
    public Slider musicSlider;
    public Slider FXSlider;


    public void ChangeState(string state)
    {
        switch (state)
        {
            case "MainMenu":
                GameManager.instance.ChangeState(GameManager.GameState.MainMenu);
                break;
            case "OptionsMenu":
                GameManager.instance.ChangeState(GameManager.GameState.OptionsMenu);
                break;
            case "StartMenu":
                GameManager.instance.ChangeState(GameManager.GameState.StartMenu);
                break;
            case "Gameplay":
                GameManager.instance.ChangeState(GameManager.GameState.Gameplay);
                break;
            case "Paused":
                GameManager.instance.ChangeState(GameManager.GameState.Paused);
                break;
            case "GameOver":
                GameManager.instance.ChangeState(GameManager.GameState.GameOver);
                break;
            default:
                Debug.Log("UI Manager detected invalid string");
                break;
        }
    }

    public void SetMapType()
    {
        //Debug.Log(mapTypeDrop.options[mapTypeDrop.value].text);

        //if (mapTypeDrop.options[mapTypeDrop.value].text == "abc")
        
    }

    public void SetPlayerNumber(int x)
    {
        if (x == 1)
        {
            GameManager.instance.numberofPlayers = 1;
        }

        else if (x == 2)
        {
            GameManager.instance.numberofPlayers = 2;
        }
    }

}
