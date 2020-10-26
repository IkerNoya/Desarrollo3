using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region VARIABLES
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
    bool canMove = true;
    Vector3 direction;
    float lerpInterpolations = 0.8f;
    float t = 0;
    float xPos;
    #endregion

    #region BASE_FUNCTIONS
    void Awake()
    {
        ParryController.parryEffect += ZoomOnPlayer;
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
        xPos = transform.position.x;
    }
    void Update()
    {
        if(crown!=null)
        {
            lastY = crown.transform.position.y;
        }

        if (transform.position.y < lastY-offset)
        {
            transform.position += direction * speed * Time.deltaTime;
            LastPos = transform.position;
        }

        if (canZoom && !canMove)
        {
            t += Time.deltaTime * lerpInterpolations;
            Vector3 middlePoint;
            middlePoint.x = player1.transform.position.x + (player2.transform.position.x - player1.transform.position.x) / 2;
            middlePoint.y = player1.transform.position.y + (player2.transform.position.y - player1.transform.position.y) / 2;
            middlePoint.z = transform.position.z;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, wantedSize, t);
            transform.position = Vector3.Lerp(transform.position, middlePoint, t);
        }
        else if(!canZoom)
        {
            t += Time.deltaTime * lerpInterpolations;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalSize, t);
            transform.position = Vector3.Lerp(transform.position, new Vector3(xPos, LastPos.y, LastPos.z), t);
            canMove = true;
        }
    }
    #endregion

    #region FUNCTIONS
    void ZoomOnPlayer(ParryController pc)
    {
        StartCoroutine(ZoomIn(1f));
    }
    #endregion

    #region COROUTINES
    IEnumerator ZoomIn(float time)
    {
        canZoom = true;
        canMove = false;
        if (player1 != null && player2 != null)
        {
            t = 0;
            canZoom = true;
            yield return new WaitForSeconds(time);
            t = 0;
            canZoom = false;
            canMove = true;
            yield return null;
        }
    }
    #endregion

    #region BASE_FUNCTIONS
    private void OnDisable()
    {
        ParryController.parryEffect -= ZoomOnPlayer;
    }
    #endregion
}
