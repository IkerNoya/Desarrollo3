using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OnClickPlay()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
