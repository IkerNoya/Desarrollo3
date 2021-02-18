using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    float musicVolume = 100;
    float sfxVolume = 100;

    public static DataManager instance;
    public static DataManager Get()
    {
       return instance;
    }

    void Awake()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        Debug.Log(musicVolume);
    }
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
}
