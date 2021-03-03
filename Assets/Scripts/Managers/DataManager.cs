using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    float musicVolume = 100;
    float sfxVolume = 100;
    public enum PlayerSelection
    {
        Nova, CyberBunny
    }
    public enum Tint
    {
        white, red, blue
    }
    public enum Hability
    {
        heal, burst, parry
    }
    [Serializable]
    public class PlayerChoice
    {
        public PlayerSelection playerSelection;
        public Tint tint;
        public Hability selectedHability;
    }
    public PlayerChoice player1Choice;
    public PlayerChoice player2Choice;
    public static DataManager instance;
    public static DataManager Instance { get { return instance; } }

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        ////default values
        //player1Choice.playerSelection = PlayerSelection.Nova;
        //player1Choice.tint = Tint.white;
        //player2Choice.playerSelection = PlayerSelection.CyberBunny;
        //player2Choice.tint = Tint.white;
    }

    void Update()
    {
    }

    #region OPTIONS_VALUES
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
    }
    public float GetSFXVolume()
    {
        return sfxVolume;
    }
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
    }
    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
    #endregion

    #region PLAYER_VALUES
    //character choice
    public void SetPlayer1Choice(PlayerSelection value)
    {
        player1Choice.playerSelection = value;
    }
    public void SetPlayer1Hability(Hability value)
    {
        player1Choice.selectedHability= value;
    }
    public PlayerSelection GetPlayer1Choice()
    {
        return player1Choice.playerSelection;
    }
    public Hability GetPlayer1Hability()
    {
        return player1Choice.selectedHability;
    }

    public void SetPlayer2Choice(PlayerSelection value)
    {
        player2Choice.playerSelection = value;
    }
    public void SetPlayer2Hability(Hability value)
    {
        player2Choice.selectedHability = value;
    }
    public PlayerSelection GetPlayer2Choice()
    {
        return player2Choice.playerSelection;
    }
    public Hability GetPlayer2Hability()
    {
        return player2Choice.selectedHability;
    }

    //character color
    public void SetPlayer1Tint(Tint value)
    {
        player1Choice.tint = value;
    }
    public Tint GetPlayer1Tint()
    {
        return player1Choice.tint;
    }

    public void SetPlayer2Tint(Tint value)
    {
        player2Choice.tint = value;
    }
    public Tint GetPlayer2Tint()
    {
        return player2Choice.tint;
    }
    #endregion

}

