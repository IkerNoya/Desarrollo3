using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject crown;
    [SerializeField] float speed;
    float lastY;

    Vector3 direction;
    void Update()
    {
        if(crown!=null)
        {
            direction = crown.transform.position - transform.position;
            lastY = crown.transform.position.y;
        }
        direction = new Vector3(0, direction.y, 0);
        if (transform.position.y < lastY) 
            transform.position += direction * speed * Time.deltaTime;
    }
}
