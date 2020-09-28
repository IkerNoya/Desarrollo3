using System;
using UnityEngine;
using UnityEngine.UI;

public class CapturePoint : MonoBehaviour
{
    [SerializeField] GameObject captureSprites;
    [SerializeField] Image captureContent;
    [SerializeField] float captureAmmount;
    bool p1Capturing;
    bool p2Capturing;

    public static event Action<CapturePoint> EndGame;
    enum State
    {
        Capturing, Blocked, Empty
    }
    State state;
    void Start()
    {
        captureContent.fillAmount = 0;
        p1Capturing = false;
        p2Capturing = false;
        state = State.Empty;
    }

    
    void Update()
    {
        switch (state)
        {
            case State.Capturing:
                captureSprites.SetActive(true);
                if (p1Capturing && p2Capturing) state = State.Blocked;
                if (!p1Capturing && !p2Capturing) state = State.Empty;
                captureContent.fillAmount += captureAmmount * Time.deltaTime;
                if (captureContent.fillAmount >= 1 && p1Capturing)
                {
                    EndGame(this);
                }
                if (captureContent.fillAmount >= 1 && p2Capturing)
                {
                    EndGame(this);
                }
                break;

            case State.Blocked:
                captureSprites.SetActive(true);
                if ((p1Capturing && !p2Capturing) || (!p1Capturing && p2Capturing)) state = State.Capturing;
                if (!p1Capturing && !p2Capturing) state = State.Empty;
                break;

            case State.Empty:
                captureSprites.SetActive(false);
                if ((p1Capturing && !p2Capturing) || (!p1Capturing && p2Capturing)) state = State.Capturing;
                captureContent.fillAmount = 0;
                break;
        }
        Debug.Log(p1Capturing);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player_1"))
        {
            p1Capturing = true;
            captureContent.color = Color.cyan;
        }
        if (collision.gameObject.CompareTag("Player_2"))
        {
            p2Capturing = true;
            captureContent.color = Color.red;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player_1"))
        {
            p1Capturing = false;
        }
        if (collision.gameObject.CompareTag("Player_2"))
        {
            p2Capturing = false;
        }
    }
}
