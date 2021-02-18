using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICredits : MonoBehaviour
{
    [SerializeField] float scrollSpeed;
    [SerializeField] Transform origin;
    [SerializeField] Transform target;
    [SerializeField] GameObject credits;
    Vector3 startingPoint;
    void Start()
    {
        if(origin!=null)
        {
            startingPoint = origin.transform.position;
        }
        if (credits != null) credits.transform.position = startingPoint;
    }
    void Update()
    {
        if(target!=null && credits!=null)
        {
            if (credits.transform.position != target.position)
            {
                credits.transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
            }
        }
    }
}
