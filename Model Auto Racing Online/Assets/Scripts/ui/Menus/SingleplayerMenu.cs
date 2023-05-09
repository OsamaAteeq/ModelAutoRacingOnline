using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using System.Collections.Generic;
using BayatGames.SaveGameFree;

public class SingleplayerMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _raceButton;

    [SerializeField] private GridLayoutGroup _container;

    [Header("Scriptable Objects :")]
    [SerializeField]
    private TournamentsList _tournamentsList;
    private List<TournamentData> _tournaments;

    [Header("Button Prefab :")]
    [SerializeField] private Button _tournamentButton;

    private string money;

    public void Start()
    {
        _tournaments = _tournamentsList.tournaments;
        foreach (TournamentData td in _tournaments)
        {
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
        _menuManager.SwitchMenu(MenuType.SingleplayerMaps);
    }
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleTournamentButtonPressed(TournamentData td)
    {
        TournamentSaver saver = new TournamentSaver();
        saver.name = td.name;
        saver.cost = td.cost;
        saver.buttonPic = td.buttonPic;
        foreach (RaceData rd in td.races)
        {
            saver.races.Add(new RaceSaver(new MapSaver(rd.map.scene_name), rd.lap, rd.opponent, rd.is_race, rd.type, rd.order, rd.difficulty, rd.income_factor, rd.cost));
        }
        SaveGame.Save<TournamentSaver>("current_tournament", saver);
        _menuManager.SwitchMenu(MenuType.OfflineTournament);
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
