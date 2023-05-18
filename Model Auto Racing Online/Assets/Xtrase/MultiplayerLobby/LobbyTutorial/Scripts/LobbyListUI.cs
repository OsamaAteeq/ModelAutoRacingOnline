using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour {


    public static LobbyListUI Instance { get; private set; }



    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] public Button refreshButton;
    [SerializeField] public Button createLobbyButton;
    [SerializeField] public Button _backButton;


    public void AwakeFunction() {
        Instance = this;

        refreshButton.onClick.AddListener(RefreshButtonClick);
        createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
    }

    public void StartFunction() {
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }

    public void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e) {
        Show();
    }

    public void LobbyManager_OnLeftLobby(object sender, EventArgs e) {
        Show();
    }

    public void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e) {
        Hide();
    }

    public void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e) {
        UpdateLobbyList(e.lobbyList);
    }

    public void UpdateLobbyList(List<Lobby> lobbyList) {
        foreach (Transform child in container) {
            if (child == lobbySingleTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList) {
            Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
            lobbySingleTransform.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
        }

    }

    public void RefreshButtonClick() {
        LobbyManager.Instance.RefreshLobbyList();
    }

    public void CreateLobbyButtonClick() {
        
        LobbyCreateUI.Instance.Show();
        _backButton.interactable = false;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

}