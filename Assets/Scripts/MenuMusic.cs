using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AK.Wwise.Event menuMusic;
    void Start()
    {
        if(menuMusic!=null)
        {
            //menuMusic.Post(Camera.main.gameObject);
        }
    }
}
