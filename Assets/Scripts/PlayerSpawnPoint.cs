using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Add this to game manager list of player spawn points on start
        GameManager.instance.playerSpawnPoints.Add(this.gameObject);
    }

    void OnDestroy()
    {
        //Remove this to game manager list of player spawn points on destroy
        GameManager.instance.playerSpawnPoints.Remove(this.gameObject);
    }
}
