using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TournamentMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Canvas _blocker;

    [SerializeField] private Button _raceButton;
    [SerializeField] private Button _timeTrialButton;
    [SerializeField] private Button _eliminationButton;

    private int intmoney;
    private string money;
    private string tournament_bought;
    private bool opened = false;

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        money = "" + PlayerPrefs.GetInt("money", 0);
        intmoney = PlayerPrefs.GetInt("money", 0);

        tournament_bought = PlayerPrefs.GetString("bought1", "no");
        if (tournament_bought == "yes" || intmoney >= 250)
        {
            if (intmoney >= 250 && tournament_bought == "no")
            {
                PlayerPrefs.SetInt("money", intmoney - 250);
                PlayerPrefs.SetString("bought1", "yes");
                money = "" + PlayerPrefs.GetInt("money", 0);
            }
            _blocker.enabled = false;
        }
        if (_blocker.enabled)
        {
            _raceButton.interactable = false;
            _eliminationButton.interactable = false;
            _timeTrialButton.interactable = false;
            _backButton.interactable = false;
            _storeButton.interactable = false;
        }
        else
        {
            _backButton.interactable = true;
            _storeButton.interactable = true;
        }
        _raceButton.interactable = false;
        _eliminationButton.interactable = false;
        _timeTrialButton.interactable = false;

        if (intmoney >= 10)
        {
            _raceButton.interactable = true;
        }
        if (intmoney >= 20)
        {
            _timeTrialButton.interactable = true;
        }
        if (intmoney >= 30)
        {
            _eliminationButton.interactable = true;
        }


        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
    }

    
    public void Update()
    {
        if (_blocker.enabled && _menuManager.GetCurrentMenu == MenuType.OfflineTournament && !opened) 
        {
            opened = true;
            StartCoroutine(waiter());

        }
    }

    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Singleplayer);

        PlayerPrefs.SetString("editable", "yes");
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
        intmoney -= 10;
        PlayerPrefs.SetInt("money", intmoney);
        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
        PlayerPrefs.SetInt("lap", 3);
        PlayerPrefs.SetString("map", "Backyard");
        PlayerPrefs.SetInt("opponent", 3);

        PlayerPrefs.SetString("type", "Race");
        PlayerPrefs.SetString("race", "yes");
        PlayerPrefs.SetString("editable", "no");
        PlayerPrefs.SetInt("factor", 5);

        _menuManager.SwitchMenu(MenuType.SingleplayerMaps);
    }
    public void HandleTimeTrialButtonPressed()
    {
        intmoney -= 20;
        PlayerPrefs.SetInt("money", intmoney);
        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
        PlayerPrefs.SetInt("lap", 2);
        PlayerPrefs.SetString("map", "Backyard");
        PlayerPrefs.SetInt("opponent", 0);

        PlayerPrefs.SetString("type", "Time");
        PlayerPrefs.SetString("race", "yes");
        PlayerPrefs.SetString("editable", "no");
        PlayerPrefs.SetInt("factor", 10);

        _menuManager.SwitchMenu(MenuType.SingleplayerMaps);
    }
    public void HandleEliminationButtonPressed()
    {
        intmoney -= 30;
        PlayerPrefs.SetInt("money", intmoney);
        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
        PlayerPrefs.SetInt("lap", 5);
        PlayerPrefs.SetString("map", "Backyard");
        PlayerPrefs.SetInt("opponent", 5);

        PlayerPrefs.SetString("type", "Elim");
        PlayerPrefs.SetString("race", "yes");
        PlayerPrefs.SetString("editable", "no");
        PlayerPrefs.SetInt("factor", 15);

        _menuManager.SwitchMenu(MenuType.SingleplayerMaps);
    }
    
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    private IEnumerator waiter()
    {
        yield return new WaitForSeconds(3f);
        opened = false;
        HandleBackButtonPressed();
        //my code here after 3 seconds
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
