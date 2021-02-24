using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] SlowMotion slowMotion;
    [SerializeField] float offset;
    [Space]
    [SerializeField] GameObject VictoryScreen_P1;
    [SerializeField] GameObject VictoryScreen_P2;
    [SerializeField] GameObject activeButtonVictoryP1;
    [SerializeField] GameObject activeButtonVictoryP2;
    [Space]
    [SerializeField] PlayerController player1;
    [SerializeField] PlayerController player2;

    void Start()
    {
        CapturePoint.VictoryP1 += EndGameEventP1;
        CapturePoint.VictoryP2 += EndGameEventP2;
        Time.timeScale = 1;
        if (VictoryScreen_P1 != null) VictoryScreen_P1.SetActive(false);
        if (VictoryScreen_P2 != null) VictoryScreen_P2.SetActive(false);
    }

    void EndGameEventP1(CapturePoint capture)
    {
        StartCoroutine(ActivateVictoryScreen(1.5f, VictoryScreen_P1, activeButtonVictoryP1));
    }
    void EndGameEventP2(CapturePoint capture)
    {
        StartCoroutine(ActivateVictoryScreen(1.5f, VictoryScreen_P2, activeButtonVictoryP2));
    }

    private void OnDisable()
    {
        CapturePoint.VictoryP1 -= EndGameEventP1;
        CapturePoint.VictoryP2 -= EndGameEventP2;
    }
    IEnumerator ActivateVictoryScreen(float time, GameObject screen, GameObject button)
    {
        StartCoroutine(slowMotion.ActivateSlowMotion(time, 0.5f));
        yield return new WaitForSeconds(time);
        Time.timeScale = 0;
        if(player1 != null) player1.SetPause(true);
        if(player2 != null) player2.SetPause(true);
        if (screen != null) screen.SetActive(true);
        if (button != null)
            EventSystem.current.SetSelectedGameObject(button);
        yield return null;
    }
}
