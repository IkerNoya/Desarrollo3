using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] Slider sfxVolume;
    [SerializeField] Slider musicVolume;
    float sfx = 0;
    float music = 0;
    void Start()
    {
        sfxVolume.value = DataManager.instance.GetSFXVolume();
        musicVolume.value = DataManager.instance.GetMusicVolume();
    }
    void Update()
    {
        sfx = sfxVolume.value;
        music = musicVolume.value;
        DataManager.instance.SetMusicVolume(music);
        DataManager.instance.SetSFXVolume(sfx);
        AkSoundEngine.SetRTPCValue("SFXVolume", sfx);
        AkSoundEngine.SetRTPCValue("MusicVolume", music);
    }
}
