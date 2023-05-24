using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;

public class AchievementMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private GridLayoutGroup _container;
    [SerializeField] private Button _ac1Button;
    [SerializeField] private Button _ac2Button;

    bool achievement1 = false;
    bool achievement2 = false;



    private string money;

    override
    public void SetEnable(int value)
    {
        base.SetEnable(value);
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        _ac1Button.gameObject.SetActive(false);
        _ac2Button.gameObject.SetActive(false);

        if (PlayerPrefs.HasKey("ac1")) 
        {
            _ac1Button.gameObject.SetActive(true);
        }
        if (PlayerPrefs.HasKey("ac2"))
        {
            _ac2Button.gameObject.SetActive(true);
        }
    }

    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Options);
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
