using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : Menu
{

    [Header("Inherit References :")]
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;

    private string username;
    private string money;
    /*
    private void Update()
    {
        
        //Testing Purpose
        PlayerPrefs.SetInt("money", 500);
        PlayerPrefs.SetString("bought1", "no");
        //Testing Purpose
        
    }*/

    override
    public void SetEnable(int value) 
    {
        base.SetEnable(value);
        username = PlayerPrefs.GetString("username", "User Name");
        _profileButton.GetComponentInChildren<TextMeshProUGUI>().text = username;
        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
    }

    

    public void HandleProfileButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
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
    public void HandlePlayButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Play);

    }
    public void HandleOptionsButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Options);
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