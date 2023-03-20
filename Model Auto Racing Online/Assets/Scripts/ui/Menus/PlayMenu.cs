using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _singleplayerButton;
    [SerializeField] private Button _multiplayerButton;
    [SerializeField] private Button _garageButton;

    private string money;

    private void Start()
    {
        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        /*OnButtonPressed(_singleplayerButton, HandleSingleplayerButtonPressed);
        OnButtonPressed(_multiplayerButton, HandleMultiplayerButtonPressed);
        OnButtonPressed(_garageButton, HandleGarageButtonPressed);

        OnButtonPressed(_backButton, HandleBackButtonPressed);
        OnButtonPressed(_storeButton, HandleStoreButtonPressed);*/
    }

    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Main);
        /*_menuManager.CloseMenu();
        _menuManager.SwitchMenu(MenuType.Level);
        */

        /* 
        StartCoroutine(ReloadLevelAsync(() =>
        {
            _menuManager.CloseMenu();
            _menuManager.SwitchMenu(MenuType.Level);
        }));

        */

        /*
        Console.WriteLine("Mode 2 Pressed");
        transform.GetComponent<Canvas>().overrideSorting = false;
        StartCoroutine(LevelLoaderAsync("MyLevel", () =>
        {
            _menuManager.CloseMenu();
            _menuManager.SwitchMenu(MenuType.Level);
        }));
        */
        /*
        _menuManager.CloseMenu();
        _menuManager.SwitchMenu(MenuType.Main);
        SceneManager.LoadSceneAsync("MyLevel");
        */
    }

    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleSingleplayerButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Singleplayer);
    }
    public void HandleMultiplayerButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Multiplayer);
    }
    public void HandleGarageButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Garage);
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
