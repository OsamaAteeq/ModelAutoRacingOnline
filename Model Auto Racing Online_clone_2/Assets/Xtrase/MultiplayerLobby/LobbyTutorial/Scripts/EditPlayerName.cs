using BayatGames.SaveGameFree;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour {


    public static EditPlayerName Instance { get; private set; }


    public event EventHandler OnNameChanged;


    [SerializeField] private TextMeshProUGUI playerNameText;


    private string playerName = "UserName";


    public void AwakeFunction() {
        Instance = this;
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player");
        playerName = player.display_name;
        GetComponent<Button>().onClick.AddListener(() => {
            UI_InputWindow.Show_Static("Player Name", playerName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 10,
            () => {
                // Cancel
            },
            (string newName) => {
                playerName = newName;

                playerNameText.text = playerName;

                OnNameChanged?.Invoke(this, EventArgs.Empty);
            });
        });

        playerNameText.text = playerName;
    }

    public void StartFunction() {
        OnNameChanged += EditPlayerName_OnNameChanged;
    }

    public void EditPlayerName_OnNameChanged(object sender, EventArgs e) {
        LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
    }

    public string GetPlayerName() {
        return playerName;
    }


}