using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] Button robotLevel;
    [SerializeField] Button cityLevel;
    [SerializeField] GameObject arrowSelectLeft;
    [SerializeField] GameObject arrowSelectRight;
    [SerializeField] GameObject frameLeft;
    [SerializeField] GameObject frameRight;

    public AK.Wwise.Event buttonSound;


    bool citySelected;
    bool robotSelected;
    void Start()
    {
        if (cityLevel != null)
            EventSystem.current.SetSelectedGameObject(cityLevel.gameObject);
    }
    void Update()
    {
        if (cityLevel != null && robotLevel != null)
        {
            if (EventSystem.current.currentSelectedGameObject == robotLevel.gameObject)
            {
                if (arrowSelectLeft != null && arrowSelectRight != null)
                {
                    arrowSelectLeft.SetActive(true);
                    arrowSelectRight.SetActive(false);
                }
                if (frameLeft != null && frameRight != null)
                {
                    frameLeft.SetActive(true);
                    frameRight.SetActive(false);
                }
                citySelected = true;
                robotSelected = false;
                Debug.Log("City");
            }
            else if (EventSystem.current.currentSelectedGameObject == cityLevel.gameObject)
            {
                if (arrowSelectLeft != null && arrowSelectRight != null)
                {
                    arrowSelectLeft.SetActive(false);
                    arrowSelectRight.SetActive(true);
                }
                if (frameLeft != null && frameRight != null)
                {
                    frameLeft.SetActive(false);
                    frameRight.SetActive(true);
                }
                robotSelected = true;
                citySelected = false;
                Debug.Log("Robot");
            }
        }

    }
    public void OnClickContinue()
    {
        if (buttonSound != null) buttonSound.Post(Camera.main.gameObject);
        if (citySelected && !robotSelected)
            SceneManager.LoadScene("CitySceneLoader");
        if (robotSelected && !citySelected)
            SceneManager.LoadScene("RobotSceneLoader");
    }
}
