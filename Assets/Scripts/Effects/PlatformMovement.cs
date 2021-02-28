using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] float speed;
    Vector3 movement;

    void Start()
    {
        
    }

    void Update()
    {
        movement = new Vector3(speed, 0);
        transform.position +=  movement * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.gameObject.layer == 17 || collision.gameObject.gameObject.layer == 18)
        {
            Debug.Log("ENTRO");
        }
    }
}
