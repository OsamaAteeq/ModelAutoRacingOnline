using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BayatGames.SaveGameFree;
using Data;

public class MainMenu : Menu
{

    [Header("Inherit References :")]
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;

    private PersonalSaver player;
    private string username;
    private string money;
    private Color color;
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
        //SaveGame.Clear();                                                         //Testing purpose only
        PersonalSaver temp = new PersonalSaver("0", "User Name", 1000, new Color(255f / 255, 189f / 255, 0));
        if (SaveGame.Exists("player"))
        {
            player = SaveGame.Load<PersonalSaver>("player", temp);
        }
        else
        {
            player = temp;
            SaveGame.Save<PersonalSaver>("player", temp);
        }
        username = player.display_name;
        money = ""+player.cash;
        color = player.color;

        _profileButton.GetComponentInChildren<TextMeshProUGUI>().text = username;
        Image[] temp2 = _profileButton.GetComponentsInChildren<Image>();
        foreach (Image t in temp2)
        {
            if (t != _profileButton.GetComponent<Image>()) 
            {
                t.color = color;
                break;
            }
        }
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