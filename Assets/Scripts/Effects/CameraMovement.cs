using System.Collections;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject Phase1Position;
    [SerializeField] GameObject Phase2Position;
    [SerializeField] GameObject Phase3Position;
    [SerializeField] GameObject Phase4Position;
    [SerializeField] float speed;
    [SerializeField] float offset;
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;

    public enum BattlePhase
    {
        phase1, phase2, phase3, phase4
    }
    public BattlePhase phase;

    public AK.Wwise.Event zoomInSound;

    Camera cam;
    float wantedSize = 4;
    float originalSize;
    float lastY;
    Vector3 lastPos;
    Vector3 lastPosBeforeParry;
    bool canZoom;
    bool canMove = true;
    Vector3 direction;
    float lerpInterpolations = 0.8f;
    float t = 0;
    float xPos;

    float player1Damage = 100;
    float player2Damage = 100;

    void Awake()
    {
        ParryController.parryEffect += ZoomOnPlayer;
    }
    void Start()
    {
        direction = Vector3.up;
        cam = Camera.main;
        originalSize = cam.orthographicSize;
        xPos = transform.position.x;
        lastPos = new Vector3(transform.position.x, transform.position.y, -10);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) phase = BattlePhase.phase1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) phase = BattlePhase.phase2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) phase = BattlePhase.phase3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) phase = BattlePhase.phase4;
            Debug.Log(lastPos.z);
    }
    void LateUpdate()
    {
        switch (phase)
        {
            case BattlePhase.phase1:
                //initial Zone
                MoveToPosition(Phase1Position);
                break;
            case BattlePhase.phase2:
                MoveToPosition(Phase2Position);
                break;
            case BattlePhase.phase3:
                MoveToPosition(Phase3Position);
                break;
            case BattlePhase.phase4:
                MoveToPosition(Phase4Position);
                break;
        }
        if (canZoom && !canMove)
        {
            zoomInSound.Post(Camera.main.gameObject);
            t += Time.deltaTime * lerpInterpolations;
            Vector3 middlePoint;
            middlePoint.x = player1.transform.position.x + (player2.transform.position.x - player1.transform.position.x) / 2;
            if (phase != BattlePhase.phase1) middlePoint.y = player1.transform.position.y + (player2.transform.position.y - player1.transform.position.y) / 2;
            else middlePoint.y = player1.transform.position.y + (player2.transform.position.y - player1.transform.position.y) / 2;
            middlePoint.z = transform.position.z;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, wantedSize, t);
            transform.position = Vector3.Lerp(transform.position, middlePoint, t);
            lastPosBeforeParry = new Vector3(lastPos.x, lastPos.y + 1, lastPos.z);
        }
        else if(!canZoom)
        {
            t += Time.deltaTime * lerpInterpolations;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalSize, t);
            transform.position = Vector3.Lerp(transform.position, new Vector3(xPos, lastPos.y, lastPos.z), t);
            canMove = true;
        }
    }
    void MoveToPosition(GameObject objective)
    {
        if(objective!=null)
            lastY = objective.transform.position.y;
        if (transform.position.y < lastY - offset)
        {
            transform.position += direction * speed * Time.deltaTime;
            lastPos = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
    void ZoomOnPlayer(ParryController pc)
    {
        StartCoroutine(ZoomIn(0.5f));
    }
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

    private void OnDisable()
    {
        ParryController.parryEffect -= ZoomOnPlayer;
    }
}
