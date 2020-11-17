using UnityEngine;
using UnityEngine.EventSystems;


public class UIControls : MonoBehaviour
{

    [SerializeField] GameObject JoystickButton;
    [SerializeField] GameObject Joystick;
    [SerializeField] GameObject Keyboard;

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            return;
        if(JoystickButton != null)
            EventSystem.current.SetSelectedGameObject(JoystickButton);
    }

    public void OnClickJoystick()
    {
        if (Joystick != null)
        {
            if (!Joystick.activeSelf)
            {
                Joystick.SetActive(true);
                if (Keyboard != null)
                Keyboard.SetActive(false);
            }
            else
                Joystick.SetActive(false);
        }
    }

    public void OnClickKeyboard()
    {
        if (Keyboard != null)
        {
            if (!Keyboard.activeSelf)
            {
                Keyboard.SetActive(true);
                if (Joystick != null)
                    Joystick.SetActive(false);
            }
            else
                Joystick.SetActive(false);
        }
    }

}
