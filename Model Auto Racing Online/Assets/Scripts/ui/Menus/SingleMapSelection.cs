using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;


public class SingleMapSelection : Menu
{
    [Serializable]
    private class Map
    {
        public string mapname;
        public Sprite picture;
        public int maxOpponents;
        public int maxLaps;
    }

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
    [SerializeField] private List<Map> maps = new List<Map>();
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
    [SerializeField] private List<string> types = new List<string>();
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

    [SerializeField] private TextMeshProUGUI _raceText;

    private string money;
    private int lap;
    private int opp;
    private string type;
    private string difficulty;
    private string order;
    private string map;
    private Map selected_map;
    private int temp = 1;
    private string editable;

    public void Start()
    {
        
    }

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        editable = PlayerPrefs.GetString("editable", "yes");
        if (editable == "no")
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


        money = "" + PlayerPrefs.GetInt("money", 0);
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        lap = PlayerPrefs.GetInt("lap", 2);
        _lapText.text = "" + lap;

        opp = PlayerPrefs.GetInt("opponent", 2);
        _opponentText.text = "" + opp;

        type = PlayerPrefs.GetString("type", "Race");
        _typeText.text = type;
        _raceText.text = type;


        difficulty = PlayerPrefs.GetString("difficulty", "Hard");
        _difficultyText.text = difficulty;

        order = PlayerPrefs.GetString("order", "Str");
        _orderText.text = order;

        map = PlayerPrefs.GetString("map", "Backyard");
        _mapText.text = map;

        foreach (Map m in maps)
        {
            if (m.mapname == map)
            {
                _mapImage.sprite = m.picture;
                selected_map = m;
            }
        }
        Debug.Log(map);
        if (selected_map.maxLaps < lap)
        {
            lap = selected_map.maxLaps;
            _lapText.text = "" + lap;
            PlayerPrefs.SetInt("lap", lap);
        }
        if (selected_map.maxOpponents < opp)
        {
            opp = selected_map.maxLaps;
            _opponentText.text = "" + opp;
            PlayerPrefs.SetInt("opponent", opp);
        }
    }
    

    public void HandlePreviousMapButtonPressed()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].mapname == map)
            {
                if (i != 0)
                {
                    Debug.Log("IFPREV");
                    map = maps[i - 1].mapname;
                    _mapImage.sprite = maps[i - 1].picture;
                    selected_map = maps[i - 1];

                }

                else
                {
                    Debug.Log("ELSEPREV");
                    _mapImage.sprite = maps[maps.Count-1].picture;
                    map = maps[maps.Count - 1].mapname;
                    selected_map = maps[maps.Count - 1];
                }
                PlayerPrefs.SetString("map", map);
                _mapText.text = map;
                foreach (Map m in maps)
                {
                    if (m.mapname == map)
                    {
                        _mapImage.sprite = m.picture;
                        selected_map = m;
                    }
                }
                if (selected_map.maxLaps < lap)
                {
                    lap = selected_map.maxLaps;
                    _lapText.text = "" + lap;
                }
                if (selected_map.maxOpponents < opp)
                {
                    opp = selected_map.maxLaps;
                    _opponentText.text = "" + opp;
                }
                break;
            }
        }
    }

    public void HandleNextMapButtonPressed()
    {
        for (int i = 0; i < maps.Count; i++) 
        {
            if (maps[i].mapname == map) 
            {
                if (i != (maps.Count - 1))
                {
                    Debug.Log("IFNEXT");
                    map = maps[i + 1].mapname;
                    _mapImage.sprite = maps[i + 1].picture;
                    selected_map = maps[i + 1];
                }

                else
                {
                    Debug.Log("ELSENEXT");
                    _mapImage.sprite = maps[0].picture;
                    map = maps[0].mapname;
                    selected_map = maps[0];
                }
                PlayerPrefs.SetString("map", map);
                _mapText.text = map;
                foreach (Map m in maps)
                {
                    if (m.mapname == map)
                    {
                        _mapImage.sprite = m.picture;
                        selected_map = m;
                    }
                }
                if (selected_map.maxLaps < lap)
                {
                    lap = selected_map.maxLaps;
                    _lapText.text = "" + lap;
                }
                if (selected_map.maxOpponents < opp)
                {
                    opp = selected_map.maxLaps;
                    _opponentText.text = "" + opp;
                }
                break;
            }
        }
    }

    public void HandleFreeRoamButtonPressed()
    {
        PlayerPrefs.SetString("race", "no");
        LoadLevel();
    }

    public void HandleRaceButtonPressed()
    {
        PlayerPrefs.SetString("race", "yes");
        LoadLevel();
    }

    private void LoadLevel()
    {
        PlayerPrefs.SetString("editable", "yes");
        if (map == "Backyard") 
        {
            Debug.Log("Backyard");
            _sceneLoader.LoadScene("track1");
            
        }
        else if(map == "SouthPark")
        {
            Debug.Log("SouthPark");
            _sceneLoader.LoadScene("South_Pacific_Town");
            /*StartCoroutine(LevelLoaderAsync(("South_Pacific_Town"),() =>
            {
                _menuManager.CloseMenu();
                _menuManager.SwitchMenu(MenuType.Main);
            }));*/
            
        }
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
                PlayerPrefs.SetString("type", type);
                _raceText.text = type;
                _typeText.text = type;
                if (type == "Time")
                {
                    temp = opp;
                    opp = 0;
                    PlayerPrefs.SetInt("opponent", opp);
                    _opponentText.text = "0";
                }
                else if (opp == 0) 
                {
                    opp = temp;
                    PlayerPrefs.SetInt("opponent", opp);
                    _opponentText.text = ""+opp;
                }
                break;
            }
        }
    }

    public void HandleNextTypeButtonPressed()
    {
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
                PlayerPrefs.SetString("type", type);
                _raceText.text = type;
                _typeText.text = type;
                if (type == "Time")
                {
                    temp = opp;
                    opp = 0;
                    PlayerPrefs.SetInt("opponent", opp);
                    _opponentText.text = "0";
                }
                else if (opp == 0)
                {
                    opp = temp;
                    PlayerPrefs.SetInt("opponent", opp);
                    _opponentText.text = "" + opp;
                }
                break;
            }
        }
    }

    public void HandlePreviousOppButtonPressed()
    {
        if (opp > 1 && type!="Time") 
        {
            opp--;
            PlayerPrefs.SetInt("opponent", opp);
            _opponentText.text = "" + opp;
        }
    }

    public void HandleNextOppButtonPressed()
    {
        if (opp < selected_map.maxOpponents && type != "Time")
        {
            opp++;
            PlayerPrefs.SetInt("opponent", opp);
            _opponentText.text = "" + opp;
        }
    }

    public void HandlePreviousLapButtonPressed()
    {
        if (lap > 1)
        {
            lap--;
            PlayerPrefs.SetInt("lap", lap);
            _lapText.text = "" + lap;
        }
    }

    public void HandleNextLapButtonPressed()
    {
        if (type == "Elim" && opp == lap) { }
        else if (lap < selected_map.maxLaps) 
        {
            lap++;
            PlayerPrefs.SetInt("lap", lap);
            _lapText.text = "" + lap;
        }
    }

    
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }

    public void HandleBackButtonPressed()
    {
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
