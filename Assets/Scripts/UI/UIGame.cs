using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIGame : MonoBehaviour
{
    [SerializeField] GameObject PauseScreen;
    [SerializeField] GameObject ResumeButton;

    [SerializeField] PlayerController p1;
    [SerializeField] PlayerController p2;

    public AK.Wwise.Event activatePauseEffect;
    public AK.Wwise.Event deactivatePauseEffect;
    public AK.Wwise.Event buttonPress;
    void Start()
    {
        PlayerController.Pause += ActivatePause;
        if(PauseScreen!=null)
            PauseScreen.SetActive(false);
    }

    void ActivatePause(PlayerController pc)
    {
        if (PauseScreen != null && !PauseScreen.activeSelf && activatePauseEffect!=null)
        {
            activatePauseEffect.Post(Camera.main.gameObject);
            Time.timeScale = 0;
            PauseScreen.SetActive(true);
            EventSystem.current.SetSelectedGameObject(ResumeButton);
        }
    }
    public void OnClickResume()
    {
        if (buttonPress != null) buttonPress.Post(Camera.main.gameObject);
        if (deactivatePauseEffect != null)
            deactivatePauseEffect.Post(Camera.main.gameObject);
        Time.timeScale = 1.0f;
        if (PauseScreen != null)
            PauseScreen.SetActive(false);
        p1.SetPause(false);
        p2.SetPause(false);
    }
    public void OnClickRestart()
    {
        if (buttonPress != null) buttonPress.Post(Camera.main.gameObject);
        if (deactivatePauseEffect != null)
            deactivatePauseEffect.Post(Camera.main.gameObject);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Game");
        if (PauseScreen != null)
            PauseScreen.SetActive(false);
        if(p1!=null) p1.SetPause(false);
        if(p2!=null) p2.SetPause(false);
    }
    public void OnClickMenu()
    {
        if (buttonPress != null) buttonPress.Post(Camera.main.gameObject);
        if (deactivatePauseEffect != null)
            deactivatePauseEffect.Post(Camera.main.gameObject);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
        if (PauseScreen != null)
            PauseScreen.SetActive(false);
        if(p1!=null) p1.SetPause(false);
        if(p2!=null) p2.SetPause(false);
    }
    public void OnClickQuit()
    {
        if (buttonPress != null) buttonPress.Post(Camera.main.gameObject);
        Application.Quit();
    }
    void OnDisable()
    {
        PlayerController.Pause -= ActivatePause;
    }
}
