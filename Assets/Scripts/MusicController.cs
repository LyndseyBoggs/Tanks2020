using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource myMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        //get AudioSource
        myMusic = GetComponent<AudioSource>();
        
        //Set volume based on game manager
        myMusic.volume = GameManager.instance.musicVolume;

        //Play music
        myMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
