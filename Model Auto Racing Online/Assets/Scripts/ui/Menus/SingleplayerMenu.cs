using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SingleplayerMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _raceButton;
    [SerializeField] private Button _tournamentButton;
    [SerializeField] private Button _tournament2Button;

    private string money;

    private void Start()
    {
        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        /*OnButtonPressed(_raceButton, HandleRaceButtonPressed);
        OnButtonPressed(_tournamentButton, HandleTournamentButtonPressed);
        OnButtonPressed(_tournament2Button, HandleTournament2ButtonPressed);

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

    public void HandleRaceButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.SingleplayerMaps);
    }
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleTournamentButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleTournament2ButtonPressed()
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
