using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [SerializeField] GameObject PauseScreen;
    [SerializeField] GameObject ResumeButton;

    [SerializeField] PlayerController p1;
    [SerializeField] PlayerController p2;

    [SerializeField] Image leftBar;
    [SerializeField] Image rightBar;

    public AK.Wwise.Event activatePauseEffect;
    public AK.Wwise.Event deactivatePauseEffect;
    public AK.Wwise.Event buttonPress;

    GameManager gm;

    void Start()
    {
        gm = GameManager.Get();
        PlayerController.Pause += ActivatePause;
        if(PauseScreen!=null)
            PauseScreen.SetActive(false);
    }

    private void Update()
    {
        leftBar.fillAmount = ConvertDamageToImageValue(gm.GetPlayer1Damage(), 0.5f);
        rightBar.fillAmount = ConvertDamageToImageValue(gm.GetPlayer2Damage(), 0.5f);
    }

    float ConvertDamageToImageValue(float value, float max)
    {
        return value * max / gm.GetInitialDamageValue();
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
