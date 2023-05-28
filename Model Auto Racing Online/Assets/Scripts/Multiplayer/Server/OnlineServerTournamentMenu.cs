using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;
using System.Collections.Generic;
using CodeMonkey.Utils;

public class OnlineServerTournamentMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _addButton;

    [SerializeField] private GridLayoutGroup _container;
    [SerializeField] private GameObject _usernamePanel;

    [Header("Button Prefab :")]
    [SerializeField] private Button _racePrefab;


    private string lobbyName;
    private LobbyManager.GameMode gameMode;
    public string LobbyName { get => lobbyName; set => lobbyName = value; }
    private bool isPrivate;
    public bool IsPrivate { get => isPrivate; set => isPrivate = value; }


    private int count;

    private TournamentSaver tournament;

    private string username = "TD";

    private Button[] buttons;

    public MapsList mapsList;

    private void Start()
    {
        if (SaveGame.Exists("online_tournament"))
        {
            tournament = SaveGame.Load<TournamentSaver>("online_tournament");
        }
        else
        {
            tournament = new TournamentSaver();
            tournament.name = username;
            SaveGame.Save<TournamentSaver>("online_tournament", tournament);
        }

        tournament = SaveGame.Load<TournamentSaver>("online_tournament");
        username = tournament.name;
        LobbyManager.Instance.Authenticate(username);
    }

    override
    public void SetEnable(int value)
    {
        _usernamePanel.SetActive(false);
        if (SaveGame.Exists("online_tournament"))
        { 
            tournament = SaveGame.Load<TournamentSaver>("online_tournament"); 
        }
        else
        {
            tournament = new TournamentSaver();
            tournament.name = username;
            SaveGame.Save<TournamentSaver>("online_tournament", tournament);
        }

        tournament = SaveGame.Load<TournamentSaver>("online_tournament");
        username = tournament.name;
        _profileButton.GetComponentInChildren<TextMeshProUGUI>().text = username;

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
        foreach (RaceSaver td in tournament.races)
        {
            //tournament.cost = td.cost;
            int rcount = mapsList.maps.Count;
            for (int fk = 0; fk < rcount; fk++)
            {
                if (td.map.name == mapsList.maps[fk].name)
                {
                    td.buttonPic = mapsList.maps[fk].map_image;
                    break;
                }
            }
        }

        foreach (RaceSaver rd in tournament.races)
        {
            count++;
            GameObject _btn = Instantiate<GameObject>(_racePrefab.gameObject, _container.transform);
            TextMeshProUGUI[] tmps = _btn.GetComponentsInChildren<TextMeshProUGUI>();

            Debug.Log(rd.buttonPic);
            _btn.GetComponent<Image>().sprite = rd.buttonPic;
           
            tmps[0].text = ""+rd.type;
            tmps[1].text = "Max: " + (rd.opponent+1) +"\n Laps: "+rd.lap;
            RaceSaver rd2 = rd;
            

            _btn.GetComponent<Button>().onClick.AddListener(delegate { HandleRaceButtonPressed(rd2); });
        }
        _container.transform.LeanSetLocalPosX((_container.cellSize.x + _container.spacing.x) * tournament.races.Count);
        buttons = _container.GetComponentsInChildren<Button>();

        base.SetEnable(value);
    }

    public void HandleProfileButtonPressed()
    {
        _usernamePanel.SetActive(true);

        _profileButton.interactable = false;
        _addButton.interactable = false;
        _backButton.interactable = false;



        Button_UI okBtn = _usernamePanel.transform.Find("okBtn").GetComponent<Button_UI>();
        Button_UI cancelBtn = _usernamePanel.transform.Find("cancelBtn").GetComponent<Button_UI>();
        TextMeshProUGUI titleText = _usernamePanel.transform.Find("titleText").GetComponent<TextMeshProUGUI>();
        TMP_InputField inputField = _usernamePanel.transform.Find("inputField").GetComponent<TMP_InputField>();

        titleText.text = "Name";
        inputField.characterLimit = 10;
        inputField.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", addedChar);
        };
        inputField.text = username;
        inputField.Select();

        okBtn.ClickFunc = () =>
        {
            _usernamePanel.SetActive(false);
            tournament.name = inputField.text;
            username = inputField.text;

            SaveGame.Save<TournamentSaver>("online_tournament", tournament);
            _profileButton.GetComponentInChildren<TextMeshProUGUI>().text = inputField.text;

            _profileButton.interactable = true;
            _profileButton.interactable = true;
            _addButton.interactable = true;
            _backButton.interactable = true;
        };

        cancelBtn.ClickFunc = () =>
        {
            _usernamePanel.SetActive(false);

            _profileButton.interactable = true;
            _addButton.interactable = true;
            _backButton.interactable = true;
        };
    }
    private char ValidateChar(string validCharacters, char addedChar)
    {
        if (validCharacters.IndexOf(addedChar) != -1)
        {
            // Valid
            return addedChar;
        }
        else
        {
            // Invalid
            return '\0';
        }
    }
    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Main);
    }
    public void HandleAddRaceButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.HostMultiplayerRace);
    }

    public void HandleRaceButtonPressed(RaceSaver race)
    {

        //ADD ALL MAPS HERE

        foreach (MapData md in mapsList.maps)
        {
            if (!LobbyManager.Instance.HasMap(md.name))
            {
                LobbyManager.Instance.AddMap(md.name, md.max_laps, md.scene_name);
                Debug.Log(md.scene_name + " Scene named " + md.name + " HAS BEEN ADDED TO LOBBY LIST");
            }
        }

            switch (race.type) 
        {
            case RaceData.RaceType.Elimination:
                gameMode = LobbyManager.GameMode.Elimination;
                break;
            case RaceData.RaceType.Race:
                gameMode = LobbyManager.GameMode.Race;
                break;
            default:
                gameMode = LobbyManager.GameMode.Race;
                break;
        }

            LobbyManager.Instance.CreateLobby(
                username,
                race.opponent + 1,
                race.MapName,
                race.SceneName,
                false,
                gameMode,
                race.lap
            );
            _menuManager.SwitchMenu(MenuType.JoinMultiplayerRace);
        
    }
}
