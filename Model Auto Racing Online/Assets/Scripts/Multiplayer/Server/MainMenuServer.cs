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

public class MainMenuServer : Menu
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    public void HandlePlayButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Play);
        
    }
    public void HandleQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
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