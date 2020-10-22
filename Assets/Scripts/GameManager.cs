using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] Transform target;
    [SerializeField] SlowMotion slowMotion;
    [SerializeField] float offset;

    Camera cam;
    float distanceToGoal;
    #endregion

    #region BASE_FUNCTIONS
    void Start()
    {
        CapturePoint.EndGame += ChangeScene;
        cam = Camera.main;
        Time.timeScale = 1;
        if(target!=null)
            distanceToGoal = target.position.y - cam.transform.position.y - offset;
    }
    void Update()
    {
    }
    #endregion

    #region FUNCTIONS
    void ChangeScene(CapturePoint capture)
    {
        StartCoroutine(EndGameEvent(1.5f));
    }
    #endregion

    #region BASE_FUNCTIONS
    private void OnDisable()
    {
        CapturePoint.EndGame -= ChangeScene;
    }
    #endregion

    #region COROUTINES
    IEnumerator EndGameEvent(float time)
    {
        StartCoroutine(slowMotion.ActivateSlowMotion(time, 0.5f));
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("End");
        yield return null;
    }
    #endregion
}
