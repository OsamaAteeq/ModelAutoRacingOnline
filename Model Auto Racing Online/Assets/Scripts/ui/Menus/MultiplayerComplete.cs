using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;

public class MultiplayerComplete : Menu
{

    [Header("Inherit External Refrences")]
    //[SerializeField] LobbyManager lobbyManager;
    [SerializeField] LobbyAssets lobbyAssets;

    [Header("Inherit Internal Refrences")]

    [SerializeField] Button _backButton;
    [SerializeField] LobbyListUI lobbyList;
    [SerializeField] LobbyUI lobbyUI;
    [SerializeField] EditPlayerName editPlayer;
    [SerializeField] LobbyCreateUI lobbyCreate;

    [SerializeField] MultiplayerHost multiplayerHost;
    [SerializeField] UI_InputWindow uI_Input;

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        editPlayer.AwakeFunction();
        lobbyList.AwakeFunction();
        lobbyUI.AwakeFunction();
        multiplayerHost.AwakeFunction();
        lobbyCreate.AwakeFunction();
        uI_Input.AwakeFunction();
        if (!LobbyManager.Instance.IsLoggedIn)
        {
            LobbyManager.Instance.Authenticate(editPlayer.GetPlayerName());
        }
        

        multiplayerHost.StartFunction();
        lobbyUI.StartFunction();
        lobbyList.StartFunction();
        //LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
    }

    public void HandleBackButtonPressed() 
    {
        LobbyManager.Instance.DeAuthenticate();
        _menuManager.SwitchMenu(MenuType.Multiplayer);
    }
}
