using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;
using System.Collections.Generic;

public class TournamentMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Canvas _blocker;
    [SerializeField] private Canvas _buyPanel;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _dontBuyButton;
    [SerializeField] private TextMeshProUGUI _optionLabel;

    [SerializeField] private GridLayoutGroup _container;

    [Header("Button Prefab :")]
    [SerializeField] private Button _raceButton;

    [Header("Scriptable Objects :")]
    [SerializeField] private TournamentsList _tournamentsList;

    private int intmoney;
    private string money;
    private bool tournament_bought = false;
    private bool opened = false;
    private TournamentData tournamentData;
    private TournamentSaver tournament;
    private InventorySaver inventory;
    private Button[] buttons;
    private PersonalSaver player;

    private bool activated = false;

    private int count = 0;
    private void Start()
    {
        
    }
    override
    public void SetEnable(int value)
    {

        activated = false;
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        intmoney = player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        _buyButton.interactable = false;
        _dontBuyButton.interactable = false;
        _buyPanel.enabled = false;

        tournament_bought = false;
        tournament = SaveGame.Load<TournamentSaver>("current_tournament");
        if (SaveGame.Exists("inventory"))
        {
            inventory = SaveGame.Load<InventorySaver>("inventory");
        }
        else 
        {
            inventory = new InventorySaver();
        }

        Debug.Log("COUNT : " + count);
        if(buttons != null)
        Debug.Log("Length : " + buttons.Length);

        for (int i = 0; i < count; i++) 
        {
            Debug.Log(buttons[i].name);
            DestroyImmediate(buttons[i].gameObject);
        }
        buttons = null;
        count = 0;
        foreach (TournamentData td in _tournamentsList.tournaments) 
        {
            if (td.name == tournament.name && td.races.Count == tournament.races.Count) 
            {
                tournamentData = td;
                //tournament.cost = td.cost;
                int rcount = td.races.Count;
                for (int fk = 0; fk < rcount; fk++) 
                {
                    
                    tournament.races[fk].buttonPic = td.races[fk].buttonPic;
                }
                break;
            }
        }
        foreach (RaceData rd in tournamentData.races)
        {
            count++;
            GameObject _btn = Instantiate<GameObject>(_raceButton.gameObject, _container.transform);
            TextMeshProUGUI[] tmps = _btn.GetComponentsInChildren<TextMeshProUGUI>();

            Debug.Log(rd.buttonPic);
            _btn.GetComponent<Image>().sprite = rd.buttonPic;
           

            int c = 0;
            foreach (TextMeshProUGUI t in tmps)
            {
                if (c == 1)
                {
                    t.text = ""+rd.type;
                }
                if (c == 0)
                {
                    t.text = "Entry " + rd.cost;
                }
                c++;
            }
            RaceSaver rd2 = RaceSaver.RaceSaverFromData(rd);
            

            _btn.GetComponent<Button>().onClick.AddListener(delegate { HandleRaceButtonPressed(rd2); });
            _btn.GetComponent<Button>().interactable = false;
        }
        _container.transform.LeanSetLocalPosX((_container.cellSize.x + _container.spacing.x) * tournament.races.Count);
        buttons = _container.GetComponentsInChildren<Button>();

        base.SetEnable(value);


        foreach (ItemSaver iis in inventory.list_items) 
        {
            if (iis.GetType() == typeof(TournamentSaver))
            {
                TournamentSaver td = (TournamentSaver)iis;

                if (td.name == tournament.name)
                {
                    tournament_bought = true;
                    break;
                }
            }
        }
        if (tournament_bought || intmoney >= tournament.cost)
        {

            _blocker.enabled = false;
            Debug.Log("Blocker Disabled");
            if (intmoney >= tournament.cost && !tournament_bought)
            {
                _buyPanel.enabled = true;
                _buyPanel.overrideSorting = true;

                Debug.Log("SEEMS OK");
                _buyButton.GetComponentInChildren<TextMeshProUGUI>().text = ""+tournament.cost;
                _backButton.interactable = false;
                _storeButton.interactable = false;

                _optionLabel.text = "Would you like to buy\n" + tournament.name;
                
                _buyButton.interactable = true;
                _dontBuyButton.interactable = true;
            }
        }
        if (_blocker.enabled || _buyPanel.enabled)
        {
            _backButton.interactable = false;
            _storeButton.interactable = false;
        }
        else
        {
            _backButton.interactable = true;
            _storeButton.interactable = true;
        }

        foreach (Button _btn in buttons)
        {
            _btn.interactable = false;
            if (!_blocker.enabled && !_buyPanel.enabled)
            {

                int cost = int.MaxValue;
                TextMeshProUGUI[] tmps = _btn.GetComponentsInChildren<TextMeshProUGUI>();
                int c = 0;
                foreach (TextMeshProUGUI t in tmps)
                {
                    if (c == 0)
                    {
                        cost = (Int32.Parse(t.text.Substring(5).Trim()));
                    }
                    c++;
                }
                if (intmoney >= cost)
                {
                    _btn.interactable = true;
                }
            }
        }
    }
    public void Update()
    {
        if (_blocker.enabled && _menuManager.GetCurrentMenu == MenuType.OfflineTournament && !opened) 
        {
            opened = true;
            StartCoroutine(waiter());
        }
        if (!_blocker.enabled && _menuManager.GetCurrentMenu == MenuType.OfflineTournament && !_buyPanel.enabled && !activated) 
        {
            activated = true;
            foreach (Button _btn in buttons)
            {
                _btn.interactable = false;
                if (!_blocker.enabled && !_buyPanel.enabled)
                {
                    int cost = int.MaxValue;
                    TextMeshProUGUI[] tmps = _btn.GetComponentsInChildren<TextMeshProUGUI>();
                    int c = 0;
                    foreach (TextMeshProUGUI t in tmps)
                    {
                        if (c == 0)
                        {
                            cost = (Int32.Parse(t.text.Substring(5).Trim()));
                        }
                        c++;
                    }
                    if (intmoney >= cost)
                    {
                        _btn.interactable = true;
                    }
                }
            }
        }
    }

    public void HandleBackButtonPressed()
    {
        _buyPanel.overrideSorting = false;
        SaveGame.Delete("current_tournament");
        _menuManager.SwitchMenu(MenuType.Singleplayer);
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

    public void HandleRaceButtonPressed(RaceSaver race)
    {

        intmoney -= race.cost;
        player.cash = intmoney;
        SaveGame.Save<PersonalSaver>("player", player);
        money = "" + intmoney;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
        race.setMapName();
        foreach (RaceData rd in tournamentData.races) 
        {
            Debug.Log("TOURNAMENT DATA : MAP : " + race.MapName);
        }
        Debug.Log("TOURNAMENT MENU : MAP : " + race.MapName);
        SaveGame.Save<RaceSaver>("current_race", race);
        _menuManager.SwitchMenu(MenuType.SingleplayerMaps);

    }
    
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleBuyButtonPressed()
    {
        PlayerPrefs.SetString("ac2","true");

        intmoney -= tournament.cost;
        player.cash = intmoney;
        SaveGame.Save<PersonalSaver>("player", player);
        money = "" + intmoney;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;


        inventory.list_items.Add(tournament);
        SaveGame.Save<InventorySaver>("inventory", inventory);

        _buyPanel.overrideSorting = false;
        _buyPanel.enabled = false;
    }
    private IEnumerator waiter()
    {
        yield return new WaitForSeconds(3f);
        opened = false;
        HandleBackButtonPressed();
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
