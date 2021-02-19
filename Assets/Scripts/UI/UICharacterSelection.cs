using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharacterSelection : MonoBehaviour
{
    [SerializeField] GameObject ContinueButton;
    [Space]
    [SerializeField] Image AvatarPlayer1;
    [SerializeField] Image AvatarPlayer2;
    [Space]
    [SerializeField] Animator avatar1Controller;
    [SerializeField] Animator avatar2Controller;
    [Space]
    [SerializeField] GameObject colorSelectionLeft;
    [SerializeField] GameObject leftWhiteButton;
    [Space]
    [SerializeField] GameObject colorSelectionRight;
    [SerializeField] GameObject rightWhiteButton;
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

        colorSelectionLeft.SetActive(false);
        colorSelectionRight.SetActive(false);

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
            data.player1Choice.playerSelection = DataManager.PlayerSelection.Nova;
            colorSelectionLeft.SetActive(true);
            EventSystem.current.SetSelectedGameObject(leftWhiteButton);
        }
        else
        {
            data.player2Choice.playerSelection = DataManager.PlayerSelection.Nova;
            colorSelectionRight.SetActive(true);
            EventSystem.current.SetSelectedGameObject(rightWhiteButton);
        }
    }
    public void OnClickCyber()
    {
        if (player == Player.player1)
        {
            data.player1Choice.playerSelection = DataManager.PlayerSelection.CyberBunny;
            colorSelectionLeft.SetActive(true);
            EventSystem.current.SetSelectedGameObject(leftWhiteButton);
        }
        else
        {
            data.player2Choice.playerSelection = DataManager.PlayerSelection.CyberBunny;
            colorSelectionRight.SetActive(true);
            EventSystem.current.SetSelectedGameObject(rightWhiteButton);
        }
    }
    public void OnClickWhite(GameObject colors)
    {
        if(player==Player.player1)
        {
            data.player1Choice.tint = DataManager.Tint.white;
            colors.SetActive(false);
        }
        else
        {
            data.player2Choice.tint = DataManager.Tint.white;
            colors.SetActive(false);
        }
        if (player <= 0)
        {
            player++;
        }
        EventSystem.current.SetSelectedGameObject(ContinueButton);
    }
    public void OnClickRed(GameObject colors)
    {
        if (player == Player.player1)
        {
            data.player1Choice.tint = DataManager.Tint.red;
            colors.SetActive(false);
        }
        else
        {
            data.player2Choice.tint = DataManager.Tint.red;
            colors.SetActive(false);
        }
        if (player <= 0)
        {
            player++;
        }
        EventSystem.current.SetSelectedGameObject(ContinueButton);

    }
    public void OnClickBlue(GameObject colors)
    {
        if (player == Player.player1)
        {
            data.player1Choice.tint = DataManager.Tint.blue;
            colors.SetActive(false);
        }
        else
        {
            data.player2Choice.tint = DataManager.Tint.blue;
            colors.SetActive(false);
        }
        if (player <= 0)
        {
            player++;
        }
        EventSystem.current.SetSelectedGameObject(ContinueButton);
    }
    public void OnHoverNova()
    {
        if (player == Player.player1)
        {
            avatar1Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/NovaAvatar") as RuntimeAnimatorController;
            AvatarPlayer1.color = visible;
        }
        else
        {
            avatar2Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/NovaAvatar") as RuntimeAnimatorController;
            AvatarPlayer2.color = visible;
        }
    }
    public void OnHoverCyberBunny()
    {
        if (player == Player.player1)
        {   
            avatar1Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/CyberAvatar") as RuntimeAnimatorController;
            AvatarPlayer1.color = visible;
        }
        else
        {
            avatar2Controller.runtimeAnimatorController = Resources.Load("Animations/UIAvatars/AnimationController/CyberAvatar") as RuntimeAnimatorController;
            AvatarPlayer2.color = visible;
        }
    }
    public void OnHoverWhite()
    {
        if(player == Player.player1)
        {
            AvatarPlayer1.color = Color.white;
        }
        else
        {
            AvatarPlayer2.color = Color.white;
        }
    }
    public void OnHoverRed()
    {
        if (player == Player.player1)
        {
            AvatarPlayer1.color = Color.red;
        }
        else
        {
            AvatarPlayer2.color = Color.red;
        }
    }
    public void OnHoveBlue()
    {
        if (player == Player.player1)
        {
            AvatarPlayer1.color = Color.blue;
        }
        else
        {
            AvatarPlayer2.color = Color.blue;
        }
    }


}
