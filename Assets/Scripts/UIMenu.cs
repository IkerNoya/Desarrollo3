using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour
{
    [SerializeField] GameObject PlayButton;
    Vector3 originalSize;
    Vector3 newSize;
    Vector3 newSizePlay;
    void Start()
    {
        newSize = new Vector3(1, 1, 1);
        newSizePlay = new Vector3(1.2f, 1.2f, 1.2f);
        EventSystem.current.SetSelectedGameObject(PlayButton);
    }
}
