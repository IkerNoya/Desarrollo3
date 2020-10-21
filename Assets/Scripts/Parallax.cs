using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float parallaxEffect;
    float startPosition;
    float length;
    float height;
    GameObject cam;
    void Start()
    {
        startPosition = transform.position.y;
        cam = Camera.main.GetComponent<GameObject>();
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        float dist = (cam.transform.position.y * parallaxEffect);
        transform.position = new Vector3(transform.position.x, startPosition + dist, transform.position.z);
    }
}
