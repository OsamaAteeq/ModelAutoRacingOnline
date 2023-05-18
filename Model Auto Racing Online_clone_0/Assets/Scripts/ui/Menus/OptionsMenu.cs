using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;

public class OptionsMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _volumeButton;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _graphicsButton;
    [SerializeField] private Button _achievmentsButton;
    [SerializeField] private Button _helpButton;
    [SerializeField] private Button _creditsButton;

    private string money;

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
    }
    public void HandleNotImplemented()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
        public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Main);

        

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
        Debug.Log("NOT IMPLEMENTED YET");
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
