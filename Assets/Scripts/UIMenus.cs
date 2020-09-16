using UnityEngine;
using UnityEngine.EventSystems;


public class UIMenus : MonoBehaviour
{
    [SerializeField] GameObject playButton;

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            return;
        EventSystem.current.SetSelectedGameObject(playButton);
    }
}
