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
    public enum Player
    {
        player1, player2
    }
    public Player player;
    
    Color visible;
    Color invisible;
    DataManager data;

    void Start()
    {
        visible = AvatarPlayer1.color;
        invisible = AvatarPlayer1.color;
        invisible.a = 0;

        AvatarPlayer1.color = invisible;
        AvatarPlayer2.color = invisible;

        data = DataManager.instance;

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
        if (player == Player.player1)
        {
            avatar1Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/NovaAvatar") as RuntimeAnimatorController;
            data.player1Choice.playerSelection = DataManager.PlayerSelection.Nova;
            AvatarPlayer1.color = visible;
        }
        else
        {
            avatar2Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/NovaAvatar") as RuntimeAnimatorController;
            data.player2Choice.playerSelection = DataManager.PlayerSelection.Nova;
            AvatarPlayer2.color = visible;
        }
        if (player == 0)
            player++;
    }
    public void OnClickCyber()
    {
        if (player == Player.player1)
        {
            avatar1Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/CyberAvatar") as RuntimeAnimatorController;
            data.player1Choice.playerSelection = DataManager.PlayerSelection.CyberBunny;
            AvatarPlayer1.color = visible;
        }
        else
        {
            avatar2Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/CyberAvatar") as RuntimeAnimatorController;
            data.player2Choice.playerSelection = DataManager.PlayerSelection.CyberBunny;
            AvatarPlayer2.color = visible;
        }
        if (player == 0)
            player++;
    }


}
