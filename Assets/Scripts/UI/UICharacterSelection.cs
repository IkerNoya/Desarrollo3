using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICharacterSelection : MonoBehaviour
{
    [SerializeField] GameObject ContinueButton;
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(ContinueButton);
    }
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(ContinueButton);
        }
        else if (EventSystem.current.currentSelectedGameObject != ContinueButton && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject);
        }
    }

}
