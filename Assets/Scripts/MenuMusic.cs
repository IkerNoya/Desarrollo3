using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AK.Wwise.Event menuMusic;
    public AK.Wwise.Event stopMusic;
    void Start()
    {
        if(menuMusic!=null && stopMusic!=null)
        {
            stopMusic.Post(Camera.main.gameObject);
            menuMusic.Post(Camera.main.gameObject);
        }
    }
}
