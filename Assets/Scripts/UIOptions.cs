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

    void Update()
    {
        sfx = sfxVolume.value;
        music = musicVolume.value;

        AkSoundEngine.SetRTPCValue("SFXVolume", sfx);
        AkSoundEngine.SetRTPCValue("MusicVolume", music);
    }
}
