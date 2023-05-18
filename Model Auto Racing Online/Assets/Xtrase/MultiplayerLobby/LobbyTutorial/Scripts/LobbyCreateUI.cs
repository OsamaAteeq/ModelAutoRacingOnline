using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {


    public static LobbyCreateUI Instance { get; private set; }


    [SerializeField] private Button createButton;
    [SerializeField] private Button lobbyNameButton;
    [SerializeField] private Button publicPrivateButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI publicPrivateText;

    [SerializeField] private MultiplayerHost multiplayerHost;


    private string lobbyName;
    private bool isPrivate;

    public void AwakeFunction() {
        Instance = this;

        createButton.onClick.AddListener(() => {
            multiplayerHost.LobbyName = lobbyName;
            multiplayerHost.IsPrivate = isPrivate;
            Hide();
            multiplayerHost.Show();
        });

        lobbyNameButton.onClick.AddListener(() => {
            UI_InputWindow.Show_Static("Lobby Name", lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
            () => {
                // Cancel
            },
            (string lobbyName) => {
                this.lobbyName = lobbyName;
                UpdateText();
            });
        });

        publicPrivateButton.onClick.AddListener(() => {
            isPrivate = !isPrivate;
            UpdateText();
        });

        

        Hide();
    }

    private void UpdateText() {
        lobbyNameText.text = lobbyName;
        publicPrivateText.text = isPrivate ? "Private" : "Public";
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);

        lobbyName = "MyLobby";
        isPrivate = false;
        //maxPlayers = 4;
        //gameMode = LobbyManager.GameMode.Race;

        UpdateText();
    }

}