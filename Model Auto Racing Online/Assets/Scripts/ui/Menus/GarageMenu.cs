using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GarageMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _nextCarButton;
    [SerializeField] private Button _previousCarButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _modifyButton;

    private string money;

    private void Start()
    {
        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        /*OnButtonPressed(_nextCarButton, HandleNextButtonPressed);
        OnButtonPressed(_previousCarButton, HandlePreviousButtonPressed);
        OnButtonPressed(_selectButton, HandleSelectButtonPressed);
        OnButtonPressed(_modifyButton, HandleModifyButtonPressed);

        OnButtonPressed(_backButton, HandleBackButtonPressed);
        OnButtonPressed(_storeButton, HandleStoreButtonPressed);*/
    }

    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Play);
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


    public void HandleNextButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandlePreviousButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleSelectButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }

    public void HandleModifyButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
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
