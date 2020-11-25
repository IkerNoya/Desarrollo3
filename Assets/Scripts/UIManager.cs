using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public AK.Wwise.Event buttonSound;

    public void ChangeScene(string scene)
    {
        if (buttonSound != null) buttonSound.Post(Camera.main.gameObject);
        SceneManager.LoadScene(scene);
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
