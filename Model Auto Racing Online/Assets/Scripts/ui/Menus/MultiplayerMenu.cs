using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;
using System.Collections.Generic;

public class MultiplayerMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _raceButton;

    [SerializeField] private GridLayoutGroup _container;

    [Header("Scriptable Objects :")]
    [SerializeField] private List<TournamentData> _tournaments = new List<TournamentData>();

    [Header("Button Prefab :")]
    [ SerializeField] private Button _tournamentButton;

    private string money;

    public void Start()
    {
        foreach (TournamentData td in _tournaments)
        {
            Debug.Log("MULTIPLAYER STARTED");
            GameObject _btn = Instantiate<GameObject>(_tournamentButton.gameObject, _container.transform);
            TextMeshProUGUI[] tmps = _btn.GetComponentsInChildren<TextMeshProUGUI>();
            _btn.GetComponent<Image>().sprite = td.buttonPic;
            int c = 0;
            foreach (TextMeshProUGUI t in tmps)
            {
                if (c == 1)
                {
                    t.text = td.name;
                }
                if (c == 0)
                {
                    t.text = "Entry " + td.cost;
                }
                c++;
            }
            TournamentData td2 = td;
            _btn.GetComponent<Button>().onClick.AddListener(delegate { HandleTournamentButtonPressed(td2); });
        }
        _container.transform.LeanSetLocalPosX((_container.cellSize.x + _container.spacing.x) * _tournaments.Count);

    }

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        SaveGame.Delete("current_tournament");
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
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

        _menuManager.SwitchMenu(MenuType.MultiplayerCompleteMenu);
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleTournamentButtonPressed(TournamentData td)
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    //LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
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
