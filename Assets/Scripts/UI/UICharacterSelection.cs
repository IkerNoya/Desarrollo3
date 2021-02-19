using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharacterSelection : MonoBehaviour
{
    [SerializeField] GameObject ContinueButton;
    [SerializeField] Image AvatarPlayer1;
    [SerializeField] Image AvatarPlayer2;
    [SerializeField] Animator avatar1Controller;
    [SerializeField] Animator avatar2Controller;
    
    Color visible;
    Color invisible;
    public enum PlayerSelection
    {
        player1, player2
    }
    public PlayerSelection playerSelection;
    void Start()
    {
        visible = AvatarPlayer1.color;
        invisible = AvatarPlayer1.color;
        invisible.a = 0;

        AvatarPlayer1.color = invisible;
        AvatarPlayer2.color = invisible;
        EventSystem.current.SetSelectedGameObject(ContinueButton);
    }
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(ContinueButton);
        }
        else if (EventSystem.current.currentSelectedGameObject != ContinueButton && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject);
        }
    }
    public void OnClickNova()
    {
        if (playerSelection == PlayerSelection.player1)
        {
            avatar1Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/NovaAvatar") as RuntimeAnimatorController;
            AvatarPlayer1.color = visible;
        }
        else
        {
            avatar2Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/NovaAvatar") as RuntimeAnimatorController;
            AvatarPlayer2.color = visible;
        }
        if(playerSelection==0)
            playerSelection++;
    }
    public void OnClickCyber()
    {
        if (playerSelection == PlayerSelection.player1)
        {
            avatar1Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/CyberAvatar") as RuntimeAnimatorController;
            AvatarPlayer1.color = visible;
        }
        else
        {
            avatar2Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/CyberAvatar") as RuntimeAnimatorController;
            AvatarPlayer2.color = visible;
        }
        if (playerSelection == 0)
            playerSelection++;
    }


}
