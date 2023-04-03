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

    [SerializeField] private GridLayoutGroup _container;

    [Header("Button Prefab :")]
    [SerializeField] private Button _raceButton;

    private int intmoney;
    private string money;
    private bool tournament_bought = false;
    private bool opened = false;
    private TournamentSaver tournament;
    private InventorySaver inventory;
    private Button[] buttons;
    private PersonalSaver player;
    private void Start()
    {
        
    }
    override
    public void SetEnable(int value)
    {
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        intmoney = player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

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
        int count = 0;
        foreach (RaceSaver rd in tournament.races)
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
            RaceSaver rd2 = rd;
            

            _btn.GetComponent<Button>().onClick.AddListener(delegate { HandleRaceButtonPressed(rd2); });
            _btn.GetComponent<Button>().interactable = false;
        }
        _container.transform.LeanSetLocalPosX((_container.cellSize.x + _container.spacing.x) * tournament.races.Count);
        buttons = _container.GetComponentsInChildren<Button>();

        base.SetEnable(value);
        

        foreach (TournamentSaver td in inventory.list_items) 
        {
            if (td == tournament) 
            {
                tournament_bought = true;
                break;
            }
        }
        if (tournament_bought || intmoney >= tournament.cost)
        {
            if (intmoney >= tournament.cost && !tournament_bought)
            {
                player.cash = (intmoney - tournament.cost);
                inventory.list_items.Add(tournament);
                money = "" + intmoney;
                SaveGame.Save<PersonalSaver>("player", player);
                SaveGame.Save<InventorySaver>("inventory", inventory);
            }
            _blocker.enabled = false;
        }
        if (_blocker.enabled)
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
            int cost = int.MaxValue;
            _btn.interactable = false;
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

        SaveGame.Save<RaceSaver>("current_race", race);

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
