using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
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


}
