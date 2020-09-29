using UnityEngine;
using UnityEngine.EventSystems;


public class UIMenus : MonoBehaviour
{
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject OptionsScreen;
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            return;
        EventSystem.current.SetSelectedGameObject(playButton);
    }
    public void OnClickOptions()
    {
        OptionsScreen.SetActive(true);
    }
    public void OnClickBack(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
