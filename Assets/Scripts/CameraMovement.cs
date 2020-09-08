using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    Vector3 movePosition;
    void Start()
    {
        movePosition = new Vector3(0f, 8f, -10f);
    }


    void Update()
    {
        if(player1.transform.localPosition.y > 4f || player2.transform.localPosition.y > 4f)
        {
            transform.position = Vector3.Lerp(transform.position, movePosition, 0.1f);
        }
    }
}
