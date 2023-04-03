using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using Data;
using BayatGames.SaveGameFree;

public class SingleMapSelection : Menu
{

    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [Header("Scene Loader :")]
    [SerializeField] private SceneLoader _sceneLoader;

    //map
    [Header("Map :")]
    [SerializeField] private Button _nextMapButton;
    [SerializeField] private Button _previousMapButton;
    [SerializeField] private TextMeshProUGUI _mapText;
    [SerializeField] private Image _mapImage;
    [SerializeField] private List<MapData> maps = new List<MapData>();
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
    
    //difficulty
    [Header("Difficulty :")]
    [SerializeField] private Button _nextDifficultyButton;
    [SerializeField] private Button _previousDifficultyButton;

    [SerializeField] private TextMeshProUGUI _difficultyText;
    //order
    [Header("Order :")]
    [SerializeField] private Button _nextOrderButton;
    [SerializeField] private Button _previousOrderButton;

    [SerializeField] private TextMeshProUGUI _orderText;

    //
    [Header("Major :")]
    [SerializeField] private Button _freeRoamButton;
    [SerializeField] private Button _raceButton;


    private string money;
    private RaceSaver race;
    private int lap;
    private int opp;
    private string type;
    private string difficulty;
    private string order;
    private string map;
    private MapSaver selected_map;
    private int temp = 1;
    private List<string> types = new List<string>();

    public void Start()
    {
        RaceData.RaceType[] raceTypes = (RaceData.RaceType[]) System.Enum.GetValues(typeof(RaceData.RaceType));
        for (int i = 0; i <raceTypes.Length; i++) 
        {
            if (raceTypes[i] == RaceData.RaceType.Elimination)
                types.Add("Elim");
            else if (raceTypes[i] == RaceData.RaceType.Race)
                types.Add("Race");
            else if (raceTypes[i] == RaceData.RaceType.TimeTrail)
                types.Add("Time");
        }
    }

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        if (SaveGame.Exists("current_tournament"))
        {
            _nextDifficultyButton.interactable = false;
            _nextLapButton.interactable = false;
            _nextMapButton.interactable = false;
            _nextOppButton.interactable = false;
            _nextOrderButton.interactable = false;
            _nextTypeButton.interactable = false;
            _previousDifficultyButton.interactable = false;
            _previousLapButton.interactable = false;
            _previousMapButton.interactable = false;
            _previousOppButton.interactable = false;
            _previousOrderButton.interactable = false;
            _previousTypeButton.interactable = false;
            _backButton.interactable = false;
            _freeRoamButton.interactable = false;
            _storeButton.interactable = false;
        }


        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
        if (SaveGame.Exists("current_race")) 
        {
            race = SaveGame.Load<RaceSaver>("current_race");
        }
        else 
        {
            MapSaver mp = new MapSaver(maps[0].scene_name);
            mp.name = maps[0].name;
            mp.max_laps = maps[0].max_laps;
            mp.max_opponents = maps[0].max_opponents;
            //mp.map_image = maps[0].map_image;
            mp.map_image = maps[0].map_image;
            race = new RaceSaver(mp,2,2,true,RaceData.RaceType.Race, RaceData.RaceOrder.Straight, RaceData.RaceDifficulty.Hard, 3f,0);
            selected_map = mp;
            SaveGame.Save<RaceSaver>("current_race", race);
        }
         
        lap = race.lap;
        _lapText.text = "" + lap;

        opp = race.opponent;
        _opponentText.text = "" + opp;

        switch (race.type) 
        {
            case RaceData.RaceType.Race:
                type = "Race";
                break;
            case RaceData.RaceType.TimeTrail:
                type = "Time";
                break;
            case RaceData.RaceType.Elimination:
                type = "Elim";
                break;
            default:
                type = "Race";
                break;

        }
        _typeText.text = type;

        switch (race.difficulty)
        {
            case RaceData.RaceDifficulty.Hard:
                difficulty = "Hard";
                break;
            default:
                difficulty = "Hard";
                break;

        }
        _difficultyText.text = difficulty;

        switch (race.order)
        {
            case RaceData.RaceOrder.Straight:
                order = "Str";
                break;
            default:
                order = "Str";
                break;

        }
        _orderText.text = order;

        
        
        selected_map = race.map;
        selected_map.name = race.map.name;
        map = selected_map.name;
        _mapText.text = race.map.name;
        //Debug.Log("MAP : " + race.map.map_image);
        // Debug.Log("MAP : " + race.map.getImage());
        //_mapImage.sprite = selected_map.map_image;
        if (race.map.map_image != null)
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
        }
        //Debug.Log(selected_map.map_image);
       // Debug.Log(selected_map.getImage());

        if (selected_map.max_laps < lap)
        {
            lap = selected_map.max_laps;
            _lapText.text = "" + lap;
            race.lap = lap;
            SaveGame.Save<RaceSaver>("current_race", race);
        }
        if (selected_map.max_opponents < opp)
        {
            opp = selected_map.max_opponents;
            _opponentText.text = "" + opp;
            race.opponent = opp;
            SaveGame.Save<RaceSaver>("current_race", race);
        }
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
                    selected_map.max_laps = maps[i - 1].max_laps;
                    selected_map.max_opponents = maps[i - 1].max_opponents;
                    //selected_map.map_image = maps[i - 1].map_image;
                    selected_map.map_image = (maps[i - 1].map_image);

                }

                else
                {
                    Debug.Log("ELSEPREV");
                    _mapImage.sprite = maps[maps.Count-1].map_image;
                    map = maps[maps.Count - 1].name;

                    selected_map.name = maps[maps.Count - 1].name;
                    selected_map.scene_name = maps[maps.Count - 1].scene_name;
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
                    _opponentText.text = "" + opp;
                    race.opponent = opp;
                }

                SaveGame.Save<RaceSaver>("current_race", race);
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
                    _opponentText.text = "" + opp;
                    race.opponent = opp;
                }

                SaveGame.Save<RaceSaver>("current_race", race);
                break;
            }
        }
    }

    public void HandleFreeRoamButtonPressed()
    {
        race.is_race = false;
        SaveGame.Save<RaceSaver>("current_race", race);
        LoadLevel();
    }

    public void HandleRaceButtonPressed()
    {
        race.is_race = false;
        SaveGame.Save<RaceSaver>("current_race", race);
        LoadLevel();
    }

    private void LoadLevel()
    {
            Debug.Log(selected_map.map_scene.name);
            _sceneLoader.LoadScene(selected_map.map_scene.name);
    }

    public void HandlePreviousOrderButtonPressed()
    {
        throw new NotImplementedException();
    }

    public void HandleNextOrderButtonPressed()
    {
        throw new NotImplementedException();
    }

    public void HandlePreviousDifficultyButtonPressed()
    {
        throw new NotImplementedException();
    }

    public void HandleNextDifficultyButtonPressed()
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
                    type = types[types.Count-1];
                }

                switch (type) 
                {
                    case "Elim":
                        selected_type = RaceData.RaceType.Elimination;
                        break;
                    case "Time":
                        selected_type = RaceData.RaceType.TimeTrail;
                        break;
                    case "Race":
                        selected_type = RaceData.RaceType.Race;
                        break;
                    default:
                        selected_type = RaceData.RaceType.Race;
                        break;
                }
                race.type = selected_type;
                _typeText.text = type;

                if (type == "Time")
                {
                    temp = opp;
                    opp = 0;
                    race.opponent = opp;
                    _opponentText.text = "0";

                    _nextOppButton.interactable = false;
                    _previousOppButton.interactable = false;
                }
                else if (opp == 0) 
                {
                    opp = temp;
                    race.opponent = opp;
                    _opponentText.text = ""+opp;

                    _nextOppButton.interactable = true;
                    _previousOppButton.interactable = true;
                }
                SaveGame.Save<RaceSaver>("current_race", race);
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
                if (i != (types.Count-1))
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
                        break;
                    case "Time":
                        selected_type = RaceData.RaceType.TimeTrail;
                        break;
                    case "Race":
                        selected_type = RaceData.RaceType.Race;
                        break;
                    default:
                        selected_type = RaceData.RaceType.Race;
                        break;
                }
                race.type = selected_type;
                _typeText.text = type;
                if (type == "Time")
                {
                    temp = opp;
                    opp = 0;
                    race.opponent = opp;
                    _opponentText.text = "0";

                    _nextOppButton.interactable = false;
                    _previousOppButton.interactable = false;
                }
                else if (opp == 0)
                {
                    opp = temp;
                    race.opponent = opp;
                    _opponentText.text = "" + opp;

                    _nextOppButton.interactable = true;
                    _previousOppButton.interactable = true;
                }
                SaveGame.Save<RaceSaver>("current_race", race);
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
            SaveGame.Save<RaceSaver>("current_race", race);
            _opponentText.text = "" + opp;
        }
    }

    public void HandleNextOppButtonPressed()
    {
        if (opp < selected_map.max_opponents)
        {
            opp++;
            race.opponent = opp;
            SaveGame.Save<RaceSaver>("current_race", race);
            _opponentText.text = "" + opp;
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
    }

    
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
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
