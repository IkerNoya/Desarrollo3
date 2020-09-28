using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapturePoint : MonoBehaviour
{
    [SerializeField] GameObject captureSprites;
    [SerializeField] Image captureContent;
    float captureTime;
    bool p1Capturing;
    bool p2Capturing;
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
        switch(state)
        {
            case State.Capturing:
                if (p1Capturing && p2Capturing) state = State.Blocked;
                if (!p1Capturing && !p2Capturing) state = State.Empty;
                captureContent.fillAmount += captureTime * Time.deltaTime;
                if(captureContent.fillAmount >= 1 && p1Capturing)
                {

                }
                if(captureContent.fillAmount >= 1 && p2Capturing)
                {

                }
                break;
            case State.Blocked:
                if ((p1Capturing && !p2Capturing) || (!p1Capturing && p2Capturing)) state = State.Capturing;
                if (!p1Capturing && !p2Capturing) state = State.Empty;
                break;
            case State.Empty:
                if ((p1Capturing && !p2Capturing) || (!p1Capturing && p2Capturing)) state = State.Capturing;
                captureContent.fillAmount = 0;
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_1"))
        {
            p1Capturing = true;
        }
        if (other.gameObject.CompareTag("player_2"))
        {
            p2Capturing = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player_1"))
        {
            p1Capturing = false;
        }
        if (other.gameObject.CompareTag("player_2"))
        {
            p2Capturing = false;
        }
    }
}
