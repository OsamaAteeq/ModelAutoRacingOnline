using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BayatGames.SaveGameFree;
using Data;
using CodeMonkey.Utils;

public class MainMenu : Menu
{

    [Header("Inherit References :")]
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;

    [Header("Profile :")]
    [SerializeField] private GameObject _usernamePanel;
    [SerializeField] private ColorsList listOfAllColors;


    private PersonalSaver player;
    private string username;
    private string money;
    private Color color;
    /*
    private void Update()
    {
        
        //Testing Purpose
        PlayerPrefs.SetInt("money", 500);
        PlayerPrefs.SetString("bought1", "no");
        //Testing Purpose
        
    }*/

    private void Start()
    {
        username = _profileButton.GetComponentInChildren<TextMeshProUGUI>().text;
        _usernamePanel.SetActive(false);
    }

    override
    public void SetEnable(int value) 
    {
        base.SetEnable(value);
        //SaveGame.Clear();                                                         //Testing purpose only
        PersonalSaver temp = new PersonalSaver("0", "User Name", 1000, new Color(255f / 255, 189f / 255, 0));
        VehicleSaver temp_vehicle = new VehicleSaver();
        if (SaveGame.Exists("player"))
        {
            player = SaveGame.Load<PersonalSaver>("player", temp);
        }
        else
        {
            player = temp;
            SaveGame.Save<PersonalSaver>("player", temp);
        }
        if (SaveGame.Exists("current_vehicle"))
        {
            temp_vehicle = SaveGame.Load<VehicleSaver>("current_vehicle");
        }
        else 
        {
            temp_vehicle.carIndex = temp_vehicle.colorsIndex = temp_vehicle.cost = temp_vehicle.motorsIndex = temp_vehicle.spoilersIndex = temp_vehicle.suspensionsIndex = temp_vehicle.wheelsIndex = 0;
            SaveGame.Save("current_vehicle", temp_vehicle);
        }
        username = player.display_name;
        money = ""+player.cash;
        Debug.Log(listOfAllColors.colors[temp_vehicle.colorsIndex].name);
        color = listOfAllColors.colors[temp_vehicle.colorsIndex].color;

        _profileButton.GetComponentInChildren<TextMeshProUGUI>().text = username;
        Image[] temp2 = _profileButton.GetComponentsInChildren<Image>();
        foreach (Image t in temp2)
        {
            if (t != _profileButton.GetComponent<Image>()) 
            {
                t.color = color;
                break;
            }
        }
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;
    }

    

    public void HandleProfileButtonPressed()
    {
        _usernamePanel.SetActive(true);

        _profileButton.interactable = false;
        _storeButton.interactable = false;

        _playButton.interactable = false;
        _optionsButton.interactable = false;
        _quitButton.interactable = false;



        Button_UI okBtn = _usernamePanel.transform.Find("okBtn").GetComponent<Button_UI>();
        Button_UI cancelBtn = _usernamePanel.transform.Find("cancelBtn").GetComponent<Button_UI>();
        TextMeshProUGUI titleText = _usernamePanel.transform.Find("titleText").GetComponent<TextMeshProUGUI>();
        TMP_InputField inputField = _usernamePanel.transform.Find("inputField").GetComponent<TMP_InputField>();

        titleText.text = "User";
        inputField.characterLimit = 10;
        inputField.onValidateInput = (string text, int charIndex, char addedChar) => {
            return ValidateChar("abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", addedChar);
        };
        inputField.text = username;
        inputField.Select();

        okBtn.ClickFunc = () => {
            _usernamePanel.SetActive(false);
            player.display_name = inputField.text;
            username = inputField.text;
            SaveGame.Save<PersonalSaver>("player", player);
            _profileButton.GetComponentInChildren<TextMeshProUGUI>().text = inputField.text;

            _profileButton.interactable = true;
            _storeButton.interactable = true;

            _playButton.interactable = true;
            _optionsButton.interactable = true;
            _quitButton.interactable = true;
        };

        cancelBtn.ClickFunc = () => {
            _usernamePanel.SetActive(false);

            _profileButton.interactable = true;
            _storeButton.interactable = true;

            _playButton.interactable = true;
            _optionsButton.interactable = true;
            _quitButton.interactable = true;
        };



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

    private void EditPlayerName_OnNameChanged(object sender, EventArgs e)
    {
        LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
    }

    public string GetPlayerName()
    {
        return username;
    }

    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandlePlayButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Play);

    }
    public void HandleOptionsButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Options);
    }
    public void HandleQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
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