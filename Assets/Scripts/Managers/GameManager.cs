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
    [Space]
    [SerializeField] float player1Damage;
    [SerializeField] float player2Damage;
    [SerializeField] CameraMovement cameraMovement;

    public static GameManager instance;
    public static GameManager Get()
    {
        return instance;
    }

    float initialDamageValue;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        initialDamageValue = player1Damage;
        CapturePoint.VictoryP1 += EndGameEventP1;
        CapturePoint.VictoryP2 += EndGameEventP2;
        Time.timeScale = 1;
        if (VictoryScreen_P1 != null) VictoryScreen_P1.SetActive(false);
        if (VictoryScreen_P2 != null) VictoryScreen_P2.SetActive(false);
    }

    void Update()
    {
        switch (cameraMovement.GetPhase())
        {
            case CameraMovement.BattlePhase.phase1:
                if (player1Damage <= initialDamageValue - (initialDamageValue / 4))
                {
                    cameraMovement.SetPhase(CameraMovement.BattlePhase.phase2);
                }
                if (player2Damage <= initialDamageValue - (initialDamageValue / 4))
                {
                    cameraMovement.SetPhase(CameraMovement.BattlePhase.phase2);
                }
                break;
            case CameraMovement.BattlePhase.phase2:
                if (player1Damage <= initialDamageValue - (initialDamageValue / 2))
                {
                    cameraMovement.SetPhase(CameraMovement.BattlePhase.phase3);
                }
                if (player2Damage <= initialDamageValue - (initialDamageValue / 2))
                {
                    cameraMovement.SetPhase(CameraMovement.BattlePhase.phase3);
                }
                break;
            case CameraMovement.BattlePhase.phase3:
                if (player1Damage <= initialDamageValue - ((initialDamageValue / 2) + (initialDamageValue / 4)))
                { 
                    cameraMovement.SetPhase(CameraMovement.BattlePhase.phase4);
                }
                if (player2Damage <= initialDamageValue - ((initialDamageValue / 2) + (initialDamageValue / 4)))
                { 
                    cameraMovement.SetPhase(CameraMovement.BattlePhase.phase4);
                }
                break;
            case CameraMovement.BattlePhase.phase4:
                //final phase
                break;
        }
    }

    void EndGameEventP1(CapturePoint capture)
    {
        StartCoroutine(ActivateVictoryScreen(1.5f, VictoryScreen_P1, activeButtonVictoryP1));
    }
    void EndGameEventP2(CapturePoint capture)
    {
        StartCoroutine(ActivateVictoryScreen(1.5f, VictoryScreen_P2, activeButtonVictoryP2));
    }

    public void Player1DamageDone(float value)
    {
        if(player1Damage>0)
            player1Damage -= value;
    }
    public void Player2DamageDone(float value)
    {
        if (player2Damage > 0)
            player2Damage -= value;
    }
    public float GetPlayer1Damage()
    {
        return player1Damage;
    }
    public float GetPlayer2Damage()
    {
        return player2Damage;
    }
    public float GetInitialDamageValue()
    {
        return initialDamageValue;
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
    private void OnDisable()
    {
        CapturePoint.VictoryP1 -= EndGameEventP1;
        CapturePoint.VictoryP2 -= EndGameEventP2;
        instance = null;
    }
    private void OnDestroy()
    {
        instance = null;
    }
}
