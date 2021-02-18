using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public void MouseOverButton(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(go);
    }
}
