using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;

public class VolumeMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _defaultButton;

    [SerializeField]
    private Slider _sound;
    [SerializeField]
    private Slider _music;

    private bool sound;
    private bool music;



    private string money;

    private void Start()
    {
        _music.onValueChanged.AddListener((v) => {
            music = (v == 1);
            PlayerPrefs.SetFloat("music",v);
            _menuManager.Music(v);
        });
        _sound.onValueChanged.AddListener((v) => {
            sound = (v == 1);
            PlayerPrefs.SetFloat("sound", v);
            AudioListener.volume = v;
        });
    }

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        float music_volume;
        float sound_volume;
        if (PlayerPrefs.HasKey("music"))
        {
            music_volume = PlayerPrefs.GetFloat("music");
        }
        else
        {
            music_volume = 1;
        }
        if (PlayerPrefs.HasKey("sound"))
        {
            sound_volume = PlayerPrefs.GetFloat("sound");
        }
        else
        {
            sound_volume = 1;
        }
        _music.value = music_volume;
        _sound.value = sound_volume;
    }

    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Options);
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


    public void HandleDefaultButtonPressed()
    {
        sound = true;
        music = true;
        _music.value = 1;
        _sound.value = 1;
        AudioListener.volume = 1;
    }
    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
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
