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

    [SerializeField] GameObject rayCircleP1;
    [SerializeField] GameObject rayP1;
    [SerializeField] GameObject rayCircleP2;
    [SerializeField] GameObject rayP2;

    Animator rayP1Anim;
    Animator rayP2Anim;

    float p1CaptureAmmount = 0;
    float p2CaptureAmmount = 0;

    public static event Action<CapturePoint> VictoryP1;
    public static event Action<CapturePoint> VictoryP2;
    void Start()
    {
        captureContent.fillAmount = 0;
        p1Capturing = false;
        p2Capturing = false;
        rayP1Anim = rayP1.GetComponent<Animator>();
        rayP2Anim = rayP2.GetComponent<Animator>();
        SetCaptureEffectState(false);
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

    void SetCaptureEffectState(bool value)
    {
        rayCircleP1.SetActive(false);
        rayP1.SetActive(false);
        rayCircleP2.SetActive(false);
        rayP2.SetActive(false);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player_1"))
        {
            if (collision.gameObject.GetComponent<PlayerController>().GetCaptureBool())
            {
                rayCircleP1.SetActive(true);
                rayP1.SetActive(true);
                p1Capturing = true;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", true);
                rayP1Anim.SetBool("HoldRay", true);
                collision.gameObject.GetComponent<PlayerController>().SetCanMove(false);
            }
            else
            {
                rayCircleP1.SetActive(false);
                rayP1.SetActive(false);
                p1Capturing = false;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", false);
                rayP1Anim.SetBool("HoldRay", false);
                collision.gameObject.GetComponent<PlayerController>().anim.SetTrigger("EndCapture");
                rayP1Anim.SetTrigger("EndRay");
            }

        }
        if (collision.gameObject.CompareTag("Player_2"))
        {
            if (collision.gameObject.GetComponent<PlayerController>().GetCaptureBool())
            {
                rayCircleP2.SetActive(true);
                rayP2.SetActive(true);
                p2Capturing = true;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", true);
                rayP2Anim.SetBool("HoldRay", true);
                collision.gameObject.GetComponent<PlayerController>().SetCanMove(false);
            }
            else 
            {
                rayCircleP2.SetActive(false);
                rayP2.SetActive(false);
                p2Capturing = false;
                collision.gameObject.GetComponent<PlayerController>().anim.SetBool("HoldCapture", false);
                rayP2Anim.SetBool("HoldRay", false);
                collision.gameObject.GetComponent<PlayerController>().anim.SetTrigger("EndCapture");
                rayP2Anim.SetTrigger("EndRay");
            }
        }
    }
}
