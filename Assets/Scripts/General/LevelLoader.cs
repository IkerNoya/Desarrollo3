using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour
{
    [SerializeField] Image slider;
    float loadingProgress;
    float timeLoading;
    public AK.Wwise.Event gameMusic;
    private void Start()
    {
        if (gameMusic != null)
        {
            gameMusic.Stop(Camera.main.gameObject);
            gameMusic.Post(Camera.main.gameObject);
        }
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        loadingProgress = 0;
        timeLoading = 0;
        yield return null;
        AsyncOperation level = null;
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("RobotSceneLoader"))
        {
            level = SceneManager.LoadSceneAsync("RobotLevel");
        }
        else if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("CitySceneLoader"))
        {
            level = SceneManager.LoadSceneAsync("CityLevel");
        }

        level.allowSceneActivation = false;

        while (!level.isDone )
        {
            timeLoading += Time.deltaTime;
            loadingProgress = level.priority + 0.1f;
            loadingProgress = loadingProgress * timeLoading*2;
             slider.fillAmount = loadingProgress;
            if(loadingProgress>=1)
            {
                level.allowSceneActivation = true;
            }
            yield return null;
        }
    }
    
}
