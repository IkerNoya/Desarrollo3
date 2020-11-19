using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] SlowMotion slowMotion;
    [SerializeField] float offset;

    Camera cam;
    float distanceToGoal;

    void Start()
    {
        CapturePoint.EndGame += ChangeScene;
        cam = Camera.main;
        Time.timeScale = 1;
        if(target!=null)
            distanceToGoal = target.position.y - cam.transform.position.y - offset;
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
        Time.timeScale = 1;
        SceneManager.LoadScene("End");
        yield return null;
    }
}
