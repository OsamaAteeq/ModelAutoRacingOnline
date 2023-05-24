using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using Data;
using BayatGames.SaveGameFree;

public class MultiplayerHost : MonoBehaviour
{
    public static MultiplayerHost Instance { get; private set; }

    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;


    //map
    [Header("Map :")]
    [SerializeField] private Button _nextMapButton;
    [SerializeField] private Button _previousMapButton;
    [SerializeField] private TextMeshProUGUI _mapText;
    [SerializeField] private Image _mapImage;
    [SerializeField] private MapsList mapsList;
    private List<MapData> maps = new List<MapData>();
    //lap
    [Header("Lap :")]
    [SerializeField] private Button _nextLapButton;
    [SerializeField] private Button _previousLapButton;
    [SerializeField] private TextMeshProUGUI _lapText;

    //opponent
    [Header("Opponents :")]
    [SerializeField] private Button _nextOppButton;
    [SerializeField] private Button _previousOppButton;
    [SerializeField] private TextMeshProUGUI _opponentText;

    //type
    [Header("Type :")]
    [SerializeField] private Button _nextTypeButton;
    [SerializeField] private Button _previousTypeButton;
    [SerializeField] private TextMeshProUGUI _typeText;
    //order
    [Header("Order :")]
    [SerializeField] private Button _nextOrderButton;
    [SerializeField] private Button _previousOrderButton;

    [SerializeField] private TextMeshProUGUI _orderText;

    //
    [Header("Major :")]
    [SerializeField] private Button _createButton;


    private string money;
    private RaceSaver race;
    private int lap;
    private int opp;
    private string type;
    private string order;
    private string map;
    private MapSaver selected_map;
    private string scene_name;

    private List<string> types = new List<string>();
    private List<string> orders = new List<string>();

    private string lobbyName;
    private bool isPrivate;
    private LobbyManager.GameMode gameMode;

    public string LobbyName { get => lobbyName; set => lobbyName = value; }
    public bool IsPrivate { get => isPrivate; set => isPrivate = value; }

    public void AwakeFunction()
    {
        Instance = this;
        maps = mapsList.maps;

        _createButton.onClick.AddListener(() => {
            PlayerPrefs.SetString("ac1", "true");
            Debug.Log("Loby Created with " + scene_name);
            LobbyManager.Instance.CreateLobby(
                lobbyName,
                (opp+1),
                map,
                scene_name,
                isPrivate,
                gameMode,
                lap
            );
            Hide();
        });

        
        RaceData.RaceType[] raceTypes = (RaceData.RaceType[])System.Enum.GetValues(typeof(RaceData.RaceType));
        RaceData.RaceDifficulty[] raceDifficulties = (RaceData.RaceDifficulty[])System.Enum.GetValues(typeof(RaceData.RaceDifficulty));
        RaceData.RaceOrder[] raceOrders = (RaceData.RaceOrder[])System.Enum.GetValues(typeof(RaceData.RaceOrder));
        for (int i = 0; i < raceTypes.Length; i++)
        {
            if (raceTypes[i] == RaceData.RaceType.Elimination)
                types.Add("Elim");
            else if (raceTypes[i] == RaceData.RaceType.Race)
                types.Add("Race");
        }
        for (int i = 0; i < raceOrders.Length; i++)
        {
            if (raceOrders[i] == RaceData.RaceOrder.Straight)
                orders.Add("Str");
            else if (raceOrders[i] == RaceData.RaceOrder.Reverse)
                orders.Add("Rev");
        }
    }

    public void StartFunction()
    {
        SaveGame.Delete("current_race");
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        MapSaver mp = new MapSaver(maps[0].scene_name);
        mp.name = maps[0].name;
        mp.max_laps = maps[0].max_laps;
        mp.max_opponents = maps[0].max_opponents;
        //mp.map_image = maps[0].map_image;
        mp.map_image = maps[0].map_image;
        int laps_to_default = (int)Math.Floor((double)(mp.max_laps / 2)) - 1;
        int opponents_to_default = (int)Math.Floor((double)(mp.max_opponents / 2)) - 1;
        if (laps_to_default < 1)
        {
            laps_to_default = 1;
        }
        if (opponents_to_default < 1)
        {
            opponents_to_default = 1;
        }
        Debug.Log("mp.scene_name : " + mp.scene_name);
        race = new RaceSaver(mp, laps_to_default, opponents_to_default, true, RaceData.RaceType.Race, RaceData.RaceOrder.Straight, RaceData.RaceDifficulty.Hard, 0f, 0);
        selected_map = mp;
        race.setMultiplayer();
        Debug.Log("selected_map.scene_name : " + selected_map.scene_name);
        SaveGame.Save<RaceSaver>("multiplayer_race", race);





        race.income_factor = 0;
        Debug.Log("TYPE : " + race.type);
        Debug.Log("diff : " + race.difficulty);
        Debug.Log("Lap : " + race.lap);
        Debug.Log("Map : " + race.map);
        Debug.Log("Order : " + race.order);
        Debug.Log("IncomeFactor : " + race.income_factor);
        Debug.Log("Oppp : " + race.opponent);
        Debug.Log("STANDINGS : " + race.standings);

        lap = race.lap;
        _lapText.text = "" + lap;

        String temp_string = "";

        opp = race.opponent;
        temp_string = "" + (opp + 1);

        _opponentText.text = temp_string;

        switch (race.type)
        {
            case RaceData.RaceType.Race:
                type = "Race";

                gameMode = LobbyManager.GameMode.Race;
                break;
            case RaceData.RaceType.Elimination:
                type = "Elim";

                gameMode = LobbyManager.GameMode.Elimination;
                break;
            default:
                type = "Race";

                gameMode = LobbyManager.GameMode.Race;
                break;

        }
        _typeText.text = type;


        switch (race.order)
        {
            case RaceData.RaceOrder.Straight:
                order = "Str";
                break;
            case RaceData.RaceOrder.Reverse:
                order = "Rev";
                break;
            default:
                order = "Str";
                break;

        }
        _orderText.text = order;

        race.setMapName();
        selected_map = race.map;



        selected_map.name = map = race.MapName;
        Debug.Log(race.MapName);
        foreach (MapData md in maps)
        {
            if (!LobbyManager.Instance.HasMap(md.name))
            {
                LobbyManager.Instance.AddMap(md.name, md.max_laps, md.scene_name);
                Debug.Log(md.scene_name + " Scene named " + md.name + " HAS BEEN ADDED TO LOBBY LIST");
            }
            if (md.name == selected_map.name)
            {
                Debug.Log("FOUND" + md.name);
                _mapText.text = race.map.name;
                _mapImage.sprite = md.map_image;
                scene_name = md.scene_name;
            }
        }


        //Debug.Log("MAP : " + race.map.map_image);
        // Debug.Log("MAP : " + race.map.getImage());
        //_mapImage.sprite = selected_map.map_image;
        /*if (race.map.map_image != null)
        {
            _mapImage.sprite = selected_map.map_image;
        }
        else
        {
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].scene_name == selected_map.scene_name)
                {
                    _mapImage.sprite = maps[i].map_image;
                }
            }
        }*/
        //Debug.Log(selected_map.map_image);
        // Debug.Log(selected_map.getImage());

        if ((selected_map.max_opponents < opp))
        {
            opp = (selected_map.max_opponents);
            _opponentText.text = "" + (opp + 1);
            race.opponent = opp;
            SaveGame.Save<RaceSaver>("multiplayer_race", race);
        }

        SaveGame.Save<RaceSaver>("multiplayer_race", race);
        /*
        Debug.Log("TYPE : " + race.type);
        Debug.Log("diff : " + race.difficulty);
        Debug.Log("Lap : " + race.lap);
        Debug.Log("Map : " + race.map);
        Debug.Log("Order : " + race.order);
        Debug.Log("IncomeFactor : " + race.income_factor);
        Debug.Log("Oppp : " + race.opponent);
        Debug.Log("STANDINGS : " + race.standings);

        Debug.Log("TYPE : " + _typeText.text);
        Debug.Log("diff : " + _difficultyText.text);
        Debug.Log("Lap : " + _lapText.text);
        Debug.Log("Map : " + _mapText.text);
        Debug.Log("Order : " + _orderText.text);
        Debug.Log("Oppp : " + _opponentText.text);
        */
    }


    public void HandlePreviousMapButtonPressed()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].scene_name == selected_map.scene_name)
            {
                if (i != 0)
                {
                    Debug.Log("IFPREV");
                    map = maps[i - 1].name;
                    _mapImage.sprite = maps[i - 1].map_image;

                    selected_map.name = maps[i - 1].name;
                    selected_map.scene_name = maps[i - 1].scene_name;
                    scene_name = maps[i - 1].scene_name;
                    selected_map.max_laps = maps[i - 1].max_laps;
                    selected_map.max_opponents = maps[i - 1].max_opponents;
                    //selected_map.map_image = maps[i - 1].map_image;
                    selected_map.map_image = (maps[i - 1].map_image);

                }

                else
                {
                    Debug.Log("ELSEPREV");
                    _mapImage.sprite = maps[maps.Count - 1].map_image;
                    map = maps[maps.Count - 1].name;

                    selected_map.name = maps[maps.Count - 1].name;
                    selected_map.scene_name = maps[maps.Count - 1].scene_name;
                    scene_name = maps[maps.Count - 1].scene_name;
                    selected_map.max_laps = maps[maps.Count - 1].max_laps;
                    selected_map.max_opponents = maps[maps.Count - 1].max_opponents;
                    //selected_map.map_image = maps[maps.Count - 1].map_image;
                    selected_map.map_image = (maps[maps.Count - 1].map_image);

                }
                race.map = selected_map;
                _mapText.text = map;

                
                if (selected_map.max_laps < lap)
                {
                    lap = selected_map.max_laps;
                    _lapText.text = "" + lap;
                    race.lap = lap;
                }

                if (selected_map.max_opponents < opp)
                {
                    opp = selected_map.max_opponents;
                    _opponentText.text = "" + (opp+1);
                    race.opponent = opp;
                }

                SaveGame.Save<RaceSaver>("multiplayer_race", race);
                break;
            }
        }
    }

    public void HandleNextMapButtonPressed()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].scene_name == selected_map.scene_name)
            {
                if (i != (maps.Count - 1))
                {
                    Debug.Log("IFNEXT");
                    map = maps[i + 1].name;
                    _mapImage.sprite = maps[i + 1].map_image;

                    selected_map.name = maps[i + 1].name;
                    selected_map.scene_name = maps[i + 1].scene_name;
                    scene_name = maps[i + 1].scene_name;
                    selected_map.max_laps = maps[i + 1].max_laps;
                    selected_map.max_opponents = maps[i + 1].max_opponents;

                    //selected_map.map_image = maps[i + 1].map_image;

                    selected_map.map_image = (maps[i + 1].map_image);

                }

                else
                {
                    Debug.Log("ELSENEXT");
                    _mapImage.sprite = maps[0].map_image;
                    map = maps[0].name;

                    selected_map.name = maps[0].name;
                    selected_map.scene_name = maps[0].scene_name;
                    scene_name = maps[0].scene_name;
                    selected_map.max_laps = maps[0].max_laps;
                    selected_map.max_opponents = maps[0].max_opponents;
                    //selected_map.map_image = maps[0].map_image;
                    selected_map.map_image = (maps[0].map_image);


                }
                race.map = selected_map;
                _mapText.text = map;

                if (selected_map.max_laps < lap)
                {
                    lap = selected_map.max_laps;
                    _lapText.text = "" + lap;
                    race.lap = lap;
                }

                if (selected_map.max_opponents < opp)
                {
                    opp = selected_map.max_opponents;
                    _opponentText.text = "" + (opp+1);
                    race.opponent = opp;
                }

                SaveGame.Save<RaceSaver>("multiplayer_race", race);
                break;
            }
        }
    }

    public void HandlePreviousLapButtonPressed()
    {
        if (lap > 1)
        {
            lap--;
            race.lap = lap;
            SaveGame.Save<RaceSaver>("current_race", race);
            _lapText.text = "" + lap;
        }

        checkLap();
    }

    public void HandleNextLapButtonPressed()
    {
        if (type == "Elim" && opp == lap) { }
        else if (lap < selected_map.max_laps)
        {
            lap++;
            race.lap = lap;
            SaveGame.Save<RaceSaver>("current_race", race);
            _lapText.text = "" + lap;
        }
        checkLap();
    }

    private void checkLap()
    {
        if (type == "Elim" && lap > opp)
        {
            lap = opp;
            race.lap = lap;
            SaveGame.Save<RaceSaver>("current_race", race);
            _lapText.text = "" + lap;
        }
    }
    private void checkOpp()
    {
        if (type == "Elim" && lap > opp)
        {
            opp = lap;
            race.opponent = opp;
            SaveGame.Save<RaceSaver>("current_race", race);
            _opponentText.text = "" + (opp+1);
        }
    }

    public void HandleRaceButtonPressed()
    {
        SaveGame.Save<RaceSaver>("multiplayer_race", race);
        Hide();
        //_menuManager.SwitchMenu(MenuType.JoinMultiplayerRace);
    }

    private void LoadLevel()
    {
        Debug.Log(selected_map.scene_name);
        //_sceneLoader.LoadScene(selected_map.scene_name);
    }

    public void HandlePreviousOrderButtonPressed()
    {
        throw new NotImplementedException();
    }

    public void HandleNextOrderButtonPressed()
    {
        throw new NotImplementedException();
    }


    public void HandlePreviousTypeButtonPressed()
    {
        RaceData.RaceType selected_type = race.type;
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i] == type)
            {
                if (i != 0)
                {
                    type = types[i - 1];
                }

                else
                {
                    type = types[types.Count - 1];
                }

                switch (type)
                {
                    case "Elim":
                        selected_type = RaceData.RaceType.Elimination;

                        gameMode = LobbyManager.GameMode.Elimination;
                        break;
                    case "Race":
                        selected_type = RaceData.RaceType.Race;

                        gameMode = LobbyManager.GameMode.Race;
                        break;
                    default:
                        selected_type = RaceData.RaceType.Race;

                        gameMode = LobbyManager.GameMode.Race;
                        break;
                }
                race.type = selected_type;
                _typeText.text = type;

                checkLap();
                SaveGame.Save<RaceSaver>("multiplayer_race", race);
                break;
            }
        }
    }

    public void HandleNextTypeButtonPressed()
    {
        RaceData.RaceType selected_type = race.type;
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i] == type)
            {
                if (i != (types.Count - 1))
                {
                    type = types[i + 1];
                }

                else
                {
                    type = types[0];
                }
                switch (type)
                {
                    case "Elim":
                        selected_type = RaceData.RaceType.Elimination;

                        gameMode = LobbyManager.GameMode.Elimination;
                        break;
                    case "Race":
                        selected_type = RaceData.RaceType.Race;

                        gameMode = LobbyManager.GameMode.Race;
                        break;
                    default:
                        selected_type = RaceData.RaceType.Race;

                        gameMode = LobbyManager.GameMode.Race;
                        break;
                }
                race.type = selected_type;
                _typeText.text = type;

                checkLap();
                SaveGame.Save<RaceSaver>("multiplayer_race", race);
                break;
            }
        }
    }

    public void HandlePreviousOppButtonPressed()
    {
        if (opp > 1)
        {
            opp--;
            race.opponent = opp;
            SaveGame.Save<RaceSaver>("multiplayer_race", race);
            _opponentText.text = "" + (opp+1);
        }

        checkOpp();
    }

    public void HandleNextOppButtonPressed()
    {
        if (opp < selected_map.max_opponents)
        {
            opp++;
            race.opponent = opp;
            SaveGame.Save<RaceSaver>("multiplayer_race", race);
            _opponentText.text = "" + (opp+1);
        }

        checkOpp();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        this.StartFunction();
    }

    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }

    public void HandleBackButtonPressed()
    {

        SaveGame.Delete("current_tournament");
        SaveGame.Save<RaceSaver>("multiplayer_race", race);
        //_menuManager.SwitchMenu(MenuType.MultiplayerRace);
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
