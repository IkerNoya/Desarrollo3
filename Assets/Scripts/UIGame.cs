using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGame : MonoBehaviour
{
    [SerializeField] GameObject PauseScreen;
    PlayerController p1;
    PlayerController p2;
    void Start()
    {
        PlayerController.Pause += ActivatePause;
        if(PauseScreen!=null)
            PauseScreen.SetActive(false);
        p1 = GameObject.FindGameObjectWithTag("Player_1").GetComponent<PlayerController>();
        p2 = GameObject.FindGameObjectWithTag("Player_2").GetComponent<PlayerController>();
    }
    
    void ActivatePause(PlayerController pc)
    {
        if (PauseScreen != null && !PauseScreen.activeSelf)
        {
            Time.timeScale = 0;
            PauseScreen.SetActive(true);
        }
    }
    public void OnClickResume()
    {
        Time.timeScale = 1.0f;
        if (PauseScreen != null)
            PauseScreen.SetActive(false);
        p1.SetPause(false);
        p2.SetPause(false);
    }
    public void OnClickRestart()
    {
        Time.timeScale = 1.0f;
        if (PauseScreen != null)
            PauseScreen.SetActive(false);
        p1.SetPause(false);
        p2.SetPause(false);
        SceneManager.LoadScene("Game");
    }
    public void OnClickMenu()
    {
        Time.timeScale = 1.0f;
        if (PauseScreen != null)
            PauseScreen.SetActive(false);
        p1.SetPause(false);
        p2.SetPause(false);
        SceneManager.LoadScene("Menu");
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
    void OnDisable()
    {
        PlayerController.Pause -= ActivatePause;
    }
}
