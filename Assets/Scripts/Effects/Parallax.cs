using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float parallaxEffect;
    [SerializeField] GameObject cam;
    float startPosition;
    float length;
    float height;
    void Start()
    {
        startPosition = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        float dist = (cam.transform.position.y * parallaxEffect);
        transform.position = new Vector3(transform.position.x, startPosition + dist, transform.position.z);
    }
}
