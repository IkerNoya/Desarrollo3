using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image map;
    [SerializeField] Transform target;
    [SerializeField] SlowMotion slowMotion;
    [SerializeField] float offset;

    Camera cam;
    float distanceToGoal;
    void Start()
    {
        CapturePoint.EndGame += ChangeScene;
        map.fillAmount = 0f;
        cam = Camera.main;
        if(target!=null)
            distanceToGoal = target.position.y - cam.transform.position.y - offset;
    }
    void Update()
    {
        if (target != null)
        {
            float currentCameraDistance = target.position.y - cam.transform.position.y - offset;
            float progress = 1f - (currentCameraDistance / distanceToGoal);
            if (cam.transform.position.y >= target.position.y - offset)
            {
                map.fillAmount = 1f;
            }
            map.fillAmount = progress;
        }
    }
    void ChangeScene(CapturePoint capture)
    {
        StartCoroutine(EndGameEvent(1.5f));
    }
    private void OnDisable()
    {
        CapturePoint.EndGame -= ChangeScene;
    }
    IEnumerator EndGameEvent(float time)
    {
        StartCoroutine(slowMotion.ActivateSlowMotion(time, 0.5f));
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("End");
        yield return null;
    }
}
