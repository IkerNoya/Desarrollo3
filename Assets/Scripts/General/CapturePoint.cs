using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CapturePoint : MonoBehaviour
{
    [SerializeField] GameObject captureSprites;
    [SerializeField] Image captureContent;
    [SerializeField] float captureAmmount;
    bool p1Capturing;
    bool p2Capturing;

    float p1CaptureAmmount = 0;
    float p2CaptureAmmount = 0;

    public static event Action<CapturePoint> VictoryP1;
    public static event Action<CapturePoint> VictoryP2;


    enum State
    {
        Capturing, Blocked
    }
    State state;
    void Start()
    {
        captureContent.fillAmount = 0;
        p1Capturing = false;
        p2Capturing = false;
        state = State.Blocked;
    }

    
    void Update()
    {
        if(p1Capturing && !p2Capturing && p2CaptureAmmount <= 0)
        {
            p1CaptureAmmount += captureAmmount * Time.deltaTime;
            captureContent.fillAmount = p1CaptureAmmount;
        }
        else if(p1CaptureAmmount > 0 && !p1Capturing && p2Capturing)
        {
            p1CaptureAmmount -= captureAmmount * Time.deltaTime;
            captureContent.fillAmount = p1CaptureAmmount;
        }
        if(!p1Capturing && p2Capturing && p1CaptureAmmount <=0)
        {
            p2CaptureAmmount += captureAmmount * Time.deltaTime;
            captureContent.fillAmount = p2CaptureAmmount;
        }
        else if (p2CaptureAmmount > 0 && p1Capturing && !p2Capturing)
        {
            p2CaptureAmmount -= captureAmmount * Time.deltaTime;
            captureContent.fillAmount = p2CaptureAmmount;
        }
        if (p1CaptureAmmount >= 1)
        {
            VictoryP1?.Invoke(this);
        }
        if (p2CaptureAmmount >= 1)
        {
            VictoryP2?.Invoke(this);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player_1"))
        {
            if (collision.gameObject.GetComponent<PlayerController>().GetCaptureBool())
            {
                p1Capturing = true;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", true);
                collision.gameObject.GetComponent<PlayerController>().SetCanMove(false);
            }
            else
            {
                p1Capturing = false;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", false);
                collision.gameObject.GetComponent<PlayerController>().anim.SetTrigger("EndCapture");
            }

        }
        if (collision.gameObject.CompareTag("Player_2"))
        {
            if (collision.gameObject.GetComponent<PlayerController>().GetCaptureBool())
            {
                p2Capturing = true;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", true);
                collision.gameObject.GetComponent<PlayerController>().SetCanMove(false);
            }
            else 
            { 
                p2Capturing = false;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", false);
                collision.gameObject.GetComponent<PlayerController>().anim.SetTrigger("EndCapture");
            }
        }
    }
}
