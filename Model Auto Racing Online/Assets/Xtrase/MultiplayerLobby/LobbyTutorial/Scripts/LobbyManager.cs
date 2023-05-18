using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour {


    public static LobbyManager Instance { get; private set; }


    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_MAP = "Map";
    public const string KEY_LAP = "Lap";
    //public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_GAME_MODE = "GameMode";



    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;

    public event EventHandler<LobbyEventArgs> OnLobbyLapChanged;
    public event EventHandler<LobbyEventArgs> OnLobbyMapChanged;
    public class LobbyEventArgs : EventArgs {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }


    public enum GameMode {
        Race,
        Elimination
    }

    private List<string> mapNames = new List<string>();
    private List<int> maxLaps = new List<int>();

    public void AddMap(string map,int max_lap) 
    {
        mapNames.Add(map);
        maxLaps.Add(max_lap);
    }
    public bool HasMap(string map)
    {
        return mapNames.Contains(map);
    }
    public bool RemoveMap(string map)
    {
        maxLaps.Remove(mapNames.IndexOf(map));
        return mapNames.Remove(map);
    }



    private float heartbeatTimer;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer = 5f;
    private Lobby joinedLobby;
    private string playerName;


    private void Awake() {
        Instance = this;
    }

    private void Update() {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public async void Authenticate(string playerName) {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    //RELAY
    public async void CreateRelay() 
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(9);
            await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (RelayServiceException e) 
        {
            Debug.LogWarning(e);
        }
    }

    public async void JoinRelay(string code) 
    {
        try
        {
            await RelayService.Instance.JoinAllocationAsync(code);
        }
        catch (RelayServiceException e) 
        {
            Debug.LogWarning(e);
        }
    }
    //RELAY

    public async void DeAuthenticate()
    {
        Debug.Log("SIGNED OUT : "+AuthenticationService.Instance.PlayerId);
        await AuthenticationService.Instance.DeleteAccountAsync();
    }

    private void HandleRefreshLobbyList() {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f) {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat() {
        if (IsLobbyHost()) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling() {
        if (joinedLobby != null) {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f) {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby()) {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }
            }
        }
    }

    public Lobby GetJoinedLobby() {
        return joinedLobby;
    }

    public bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby() {
        if (joinedLobby != null && joinedLobby.Players != null) {
            foreach (Player player in joinedLobby.Players) {
                if (player.Id == AuthenticationService.Instance.PlayerId) {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    private Player GetPlayer() {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            //{ KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Marine.ToString()) }
        });
    }

    public void ChangeGameMode() {
        if (IsLobbyHost()) {
            int lap =
              Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
            GameMode gameMode =
                Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);

            switch (gameMode) {
                default:
                case GameMode.Elimination:
                    gameMode = GameMode.Race;
                    break;
                case GameMode.Race:
                    gameMode = GameMode.Elimination;
                    break;
            }
            if (gameMode == GameMode.Elimination && lap > joinedLobby.MaxPlayers) 
            {
                lap = joinedLobby.MaxPlayers;
            }

            UpdateLobbyGameMode(gameMode);
            UpdateLobbyLap("" + lap);
        }
    }

    public void IncreaseLap()
    {
        if (IsLobbyHost())
        {
            GameMode gameMode =
                Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);
            int lap =
               Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
            string map = joinedLobby.Data[KEY_MAP].Value;
            int index = 0;
            for (int i = 0; i < maxLaps.Count; i++)
            {
                if (map == mapNames[i])
                {
                    index = i;
                    break;
                }
            }
            if (lap < maxLaps[index])
            {
                if (gameMode == GameMode.Elimination && lap == joinedLobby.MaxPlayers)
                { }
                else
                {
                    lap = lap + 1;
                }
            }
            UpdateLobbyLap("" + lap);
        }
    }

    public bool isStartable()
    {
        if (IsLobbyHost())
        {
            bool check1 = false;
            bool check2 = false;
            int count = joinedLobby.Players.Count;
            int lap =
                   Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
            GameMode gameMode =
                    Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);
            if (gameMode == GameMode.Elimination && (count >= 2))
            {
                if (count > lap)
                    check1 = true;
            }
            else if (count >= 2)
            {
                check2 = true;
            }

            return (check1 && check2);
        }
        return false;
    }

    public void DecreaseLap()
    {
        if (IsLobbyHost())
        {
            int lap =
               Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
            if (lap > 1)
            {
                lap = lap - 1;
            }

            UpdateLobbyLap("" + lap);

        }
    }

    public void ChangeMap()
    {
        if (IsLobbyHost())
        {
            string map = joinedLobby.Data[KEY_MAP].Value;
            int lap =
               Int32.Parse(joinedLobby.Data[KEY_LAP].Value);

            for (int i = 0; i < mapNames.Count; i++)
            {
                if (mapNames[i] == map)
                {
                    if (i != (mapNames.Count - 1))
                    {
                        map = mapNames[i + 1];
                        if (lap > maxLaps[i + 1]) 
                        {
                            lap = maxLaps[i + 1];
                        }
                    }
                    else 
                    {
                        map = mapNames[0];
                        if (lap > maxLaps[0])
                        {
                            lap = maxLaps[0];
                        }
                    }
                    break;
                }
            }
            UpdateLobbyLap("" + lap);
            UpdateLobbyMap(map);
        }
    }

    

    public async void CreateLobby(string lobbyName, int maxPlayers, string map, bool isPrivate, GameMode gameMode, int lap) {
        Player player = GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions {
            Player = player,
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject> {
                { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) },
                { KEY_MAP, new DataObject(DataObject.VisibilityOptions.Public, map) },
                { KEY_LAP, new DataObject(DataObject.VisibilityOptions.Public, ""+lap) }
            }
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

        Debug.Log("Created Lobby " + lobby.Name);
    }

    public async void RefreshLobbyList() {
        try {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions {
            Player = player
        });

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void JoinLobby(Lobby lobby) {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
            Player = player
        });

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void UpdatePlayerName(string playerName) {
        this.playerName = playerName;

        if (joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    
    public async void QuickJoinLobby() {
        try {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void UpdateLobbyGameMode(GameMode gameMode) {
        try {
            Debug.Log("UpdateLobbyGameMode " + gameMode);
            
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
                }
            });

            joinedLobby = lobby;

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    public async void UpdateLobbyLap(string lap)
    {
        try
        {
            Debug.Log("UpdateLobbyLap " + lap);

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_LAP, new DataObject(DataObject.VisibilityOptions.Public, ""+lap) }
                }
            });

            joinedLobby = lobby;

            OnLobbyLapChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void UpdateLobbyMap(string map)
    {
        try
        {
            Debug.Log("UpdateLobbyMap " + map);

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_MAP, new DataObject(DataObject.VisibilityOptions.Public, map) }
                }
            });

            joinedLobby = lobby;

            OnLobbyMapChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}