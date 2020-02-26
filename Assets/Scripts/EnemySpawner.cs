using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Add this spawner to the game manager list of enemy spawn points
        GameManager.instance.enemySpawnPoints.Add(this.transform);
    }

    void OnDestroy()
    {
        //Remove this spawner to the game manager list of enemy spawn points
        GameManager.instance.enemySpawnPoints.Remove(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
