using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] Slider sfxVolume;
    [SerializeField] Slider musicVolume;
    float sfx = 0;
    float volume = 0;

    void Update()
    {
        sfx = sfxVolume.value;
        volume = musicVolume.value;

        AkSoundEngine.SetRTPCValue("Subir_BajarFX", sfx);
        AkSoundEngine.SetRTPCValue("Subir_BajarVolumen", volume);
    }
}
