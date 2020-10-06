using UnityEngine;
using UnityEngine.EventSystems;


public class UIMenus : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject OptionsScreen;
    #endregion

    #region BASE_FUNCTIONS
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            return;
        EventSystem.current.SetSelectedGameObject(playButton);
    }
    #endregion

    #region FUNCTIONS
    public void OnClickOptions()
    {
        OptionsScreen.SetActive(true);
    }
    public void OnClickBack(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    #endregion
}
