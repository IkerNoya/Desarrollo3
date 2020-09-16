using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject crown;
    [SerializeField] float speed;
    [SerializeField] float offset;
    float lastY;

    Vector3 direction;
    private void Start()
    {
        if (crown != null)
        {
            direction = crown.transform.position - transform.position;
            direction = new Vector3(0, direction.y, 0);
        }
    }
    void Update()
    {
        if(crown!=null)
        {
            lastY = crown.transform.position.y;
        }
        if (transform.position.y < lastY-offset) 
            transform.position += direction * speed * Time.deltaTime;
    }
}
