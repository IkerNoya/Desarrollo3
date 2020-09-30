using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject crown;
    [SerializeField] float speed;
    [SerializeField] float offset;
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    Camera cam;
    float wantedSize = 4;
    float originalSize;
    float lastY;
    Vector3 LastPos;
    bool canZoom;
    Vector3 direction;
    void Awake()
    {
        PlayerController.Zoom += ZoomOnPlayer;
    }
    private void Start()
    {
        if (crown != null)
        {
            direction = crown.transform.position - transform.position;
            direction = new Vector3(0, direction.y, 0);
        }
        cam = Camera.main;
        originalSize = cam.orthographicSize;
    }
    void Update()
    {
        if(crown!=null)
        {
            lastY = crown.transform.position.y;
        }

        if (transform.position.y < lastY-offset) 
            transform.position += direction * speed * Time.deltaTime;
                    
        if(!canZoom) LastPos = transform.position;
    }
    void ZoomOnPlayer(PlayerController pc)
    {
        StartCoroutine(ZoomIn(1f));
    }

    IEnumerator ZoomIn(float time)
    {
        canZoom = true;
        if (player1 != null && player2 != null)
        {
            Vector3 middlePoint;
            middlePoint.x = player1.transform.position.x + (player2.transform.position.x - player1.transform.position.x) / 2;
            middlePoint.y = player1.transform.position.y + (player2.transform.position.y - player1.transform.position.y) / 2;
            middlePoint.z = transform.position.z;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, wantedSize, 0.8f);
            transform.position = Vector3.Lerp(transform.position, middlePoint, 0.8f);
            yield return new WaitForSeconds(time);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalSize, 0.8f);
            transform.position = Vector3.Lerp(transform.position, LastPos, 0.8f);
            yield return null;
        }
    }

    private void OnDisable()
    {
        PlayerController.Zoom -= ZoomOnPlayer;
    }
}
