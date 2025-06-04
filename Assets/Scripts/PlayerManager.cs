using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public bool isPaused = false;
    public Player player;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
       // player.enableFPS(false); //THIS PROBABLY NEEDS TO BE FIXED AT SOME POINT DUMBASS DONT FORGET THIS YOU STUPID ASSHOLE
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        //player.enableFPS(true);
    }
}
