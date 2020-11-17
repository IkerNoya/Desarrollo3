using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        Time.timeScale = 1; //take out when i find out what the fuck is happening to the time scale when the match starts
        SceneManager.LoadScene(scene);
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
