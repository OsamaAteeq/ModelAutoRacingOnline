using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {


    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI gameModeText;

    [SerializeField] private TextMeshProUGUI lapSettingText;
    [SerializeField] private Button goButton;

    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI mapText;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button changeGameModeButton;

    [SerializeField] private Button increaseLapButton;
    [SerializeField] private Button decreaseLapButton;
    [SerializeField] private Button changeMapButton;

    [SerializeField] private Button _backButton;


    public void AwakeFunction() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);


        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
            _backButton.interactable = true;
        });

        changeGameModeButton.onClick.AddListener(() => {
            LobbyManager.Instance.ChangeGameMode();
        });
        increaseLapButton.onClick.AddListener(() => {
            LobbyManager.Instance.IncreaseLap();
        });
        decreaseLapButton.onClick.AddListener(() => {
            LobbyManager.Instance.DecreaseLap();
        }); 
        changeMapButton.onClick.AddListener(() => {
            LobbyManager.Instance.ChangeMap();
        });

        goButton.onClick.AddListener(()=> {
            LobbyManager.Instance.StartGame();
        });
    }

    public void StartFunction() {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
        LobbyManager.Instance.OnLobbyMapChanged += UpdateLobby_Event;
        LobbyManager.Instance.OnLobbyLapChanged += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;

        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();

        foreach (Player player in lobby.Players) {
            Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
            playerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                LobbyManager.Instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
            );

            lobbyPlayerSingleUI.UpdatePlayer(player);
            Rect r = lobbyPlayerSingleUI.GetComponent<RectTransform>().rect;
            r.width = Screen.width;
        }

        changeGameModeButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

        changeMapButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());
        increaseLapButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

        decreaseLapButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());
        goButton.gameObject.SetActive(LobbyManager.Instance.isStartable());

        lapSettingText.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        gameModeText.text = lobby.Data[LobbyManager.KEY_GAME_MODE].Value;
        mapText.text = lobby.Data[LobbyManager.KEY_MAP].Value;

        lapText.text = "Laps: "+lobby.Data[LobbyManager.KEY_LAP].Value;

        Show();
    }

    private void ClearLobby() {
        foreach (Transform child in container) {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}