using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BayatGames.SaveGameFree;
using Data;
using CodeMonkey.Utils;
using Unity.Services.Authentication;
using Unity.Netcode;

public class ServerLobbyMenu : Menu
{
    public LobbyUI lobbyUI;
    override
    public void SetEnable(int value)
    {
        lobbyUI.AwakeFunction();
        lobbyUI.StartFunction();

        base.SetEnable(value);
    }

    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Play);
    }
    #region Event Handler

    public void loadGameScene() 
    {
        SceneManager.LoadScene("Game");
    }
    IEnumerator LevelLoaderAsync(string name, Action OnSceneLoaded = null)
    {
        yield return SceneManager.LoadSceneAsync(name);
        OnSceneLoaded?.Invoke();
    }
    IEnumerator ReloadLevelAsync(Action OnSceneLoaded = null)
    {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        OnSceneLoaded?.Invoke();
    }
    #endregion
}