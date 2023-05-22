using BayatGames.SaveGameFree;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
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
    public const string KEY_SCENE_NAME = "Scene";
    public const string KEY_LAP = "Lap";
    public const string KEY_START_GAME = "0";
    
    //public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_GAME_MODE = "GameMode";



    public event EventHandler OnLeftLobby;
    public event EventHandler OnGameStarted;

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

    private List<string> sceneNames = new List<string>();
    private List<int> maxLaps = new List<int>();
    private RaceSaver raceSaver;
    private bool isLoggedIn = false;
    public bool IsLoggedIn { get => isLoggedIn; }
    public void AddMap(string map,int max_lap,string sceneName) 
    {
        sceneNames.Add(sceneName);
        mapNames.Add(map);
        maxLaps.Add(max_lap);
    }
    public bool HasMap(string map)
    {
        return mapNames.Contains(map);
    }
    public bool RemoveMap(string map)
    {
        maxLaps.RemoveAt(mapNames.IndexOf(map));
        sceneNames.RemoveAt(mapNames.IndexOf(map));
        return mapNames.Remove(map);
    }


    public async void StartGame()
    {
        if (isStartable()) 
        {

            try { 
                Debug.Log("START MULTIPLAYER");
                string relayCode = await CreateRelay();



                Debug.Log("Trying to load: " + joinedLobby.Data[KEY_SCENE_NAME].Value);
                NetworkManager.Singleton.SceneManager.LoadScene(joinedLobby.Data[KEY_SCENE_NAME].Value, UnityEngine.SceneManagement.LoadSceneMode.Single);
                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject> {
                        {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    } 
                });
                joinedLobby = lobby;
                
            } 
            catch (LobbyServiceException e) 
            {
                Debug.LogWarning(e);
            }
        }
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
    public bool HasLobby() 
    {
        return (joinedLobby != null);
    }
    public async void Authenticate(string playerName) {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
            isLoggedIn = true;
            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    //RELAY
    public async Task<string> CreateRelay() 
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(9);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            if (SaveGame.Exists("multiplayer_race"))
            {
                raceSaver = SaveGame.Load<RaceSaver>("multiplayer_race");

                raceSaver.lap = Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
                GameMode gameMode =
                    Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);
                RaceData.RaceType raceType = RaceData.RaceType.Race;
                switch (gameMode)
                {
                    case GameMode.Race:
                        raceType = RaceData.RaceType.Race;
                        break;
                    case GameMode.Elimination:
                        raceType = RaceData.RaceType.Elimination;
                        break;
                    default:
                        raceType = RaceData.RaceType.Race;
                        break;
                }
                raceSaver.type = raceType;
                raceSaver.lap = Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
                raceSaver.opponent = (joinedLobby.Players.Count);
                SaveGame.Save<RaceSaver>("multiplayer_race",raceSaver);
            }
            else 
            {
                int laps = Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
                GameMode gameMode =
                    Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);
                RaceData.RaceType raceType = RaceData.RaceType.Race;
                switch (gameMode)
                {
                    case GameMode.Race:
                        raceType = RaceData.RaceType.Race;
                        break;
                    case GameMode.Elimination:
                        raceType = RaceData.RaceType.Elimination;
                        break;
                    default:
                        raceType = RaceData.RaceType.Race;
                        break;
                }
                string map_name = joinedLobby.Data[KEY_MAP].Value;
                string scene_name = joinedLobby.Data[KEY_SCENE_NAME].Value;
                int opp = (joinedLobby.MaxPlayers - 1);
                raceSaver = new RaceSaver(map_name, scene_name, laps, opp, raceType);
                SaveGame.Save<RaceSaver>("multiplayer_race", raceSaver);
            }
            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e) 
        {
            Debug.LogWarning(e);
        }
        return null;
    }

    public async void JoinRelay(string code) 
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e) 
        {
            Debug.LogWarning(e);
        }
    }
    //RELAY

    public async void DeAuthenticate()
    {
        
        AuthenticationService.Instance.SignedOut += () => 
        {
            Debug.Log("SIGNED OUT : " + AuthenticationService.Instance.PlayerId);
            isLoggedIn = false;
        };
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
                if (joinedLobby.Data[KEY_START_GAME].Value != "0") 
                {
                    if (!IsLobbyHost()) 
                    {
                        JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                    }
                    joinedLobby = null;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
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
            if (gameMode == GameMode.Elimination && lap >= joinedLobby.MaxPlayers) 
            {
                lap = joinedLobby.MaxPlayers-1;
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
                if (gameMode == GameMode.Elimination && lap >= (joinedLobby.MaxPlayers-1))
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
            bool check1 = true;
            bool check2 = true;
            int count = joinedLobby.Players.Count;
            int lap =
                   Int32.Parse(joinedLobby.Data[KEY_LAP].Value);
            GameMode gameMode =
                    Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);
            if (gameMode == GameMode.Elimination)
            {
                if (count <= lap)
                    check1 = false;
                else
                    check1 = true;
            }
            if (count < 2)
            {
                check2 = false;
            }
            else 
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
            string scene_name = joinedLobby.Data[KEY_SCENE_NAME].Value;
            int lap =
               Int32.Parse(joinedLobby.Data[KEY_LAP].Value);

            for (int i = 0; i < mapNames.Count; i++)
            {
                if (mapNames[i] == map)
                {
                    if (i != (mapNames.Count - 1))
                    {
                        map = mapNames[i + 1];
                        scene_name = sceneNames[i + 1];
                        if (lap > maxLaps[i + 1]) 
                        {
                            lap = maxLaps[i + 1];
                        }
                    }
                    else 
                    {
                        map = mapNames[0];
                        scene_name = sceneNames[0];
                        if (lap > maxLaps[0])
                        {
                            lap = maxLaps[0];
                        }
                    }
                    break;
                }
            }
            UpdateLobbyLap("" + lap);
            UpdateLobbyMap(map,scene_name);
        }
    }

    

    public async void CreateLobby(string lobbyName, int maxPlayers, string map, string scene_name, bool isPrivate, GameMode gameMode, int lap) {
        Player player = GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions {
            Player = player,
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject> {
                { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) },
                { KEY_MAP, new DataObject(DataObject.VisibilityOptions.Public, map) },
                { KEY_SCENE_NAME, new DataObject(DataObject.VisibilityOptions.Public, scene_name) },
                { KEY_LAP, new DataObject(DataObject.VisibilityOptions.Public, ""+lap) },
                { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")} 
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
    public async void UpdateLobbyMap(string map, string scene_name)
    {
        try
        {
            Debug.Log("UpdateLobbyMap " + map);

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_MAP, new DataObject(DataObject.VisibilityOptions.Public, map) },
                    { KEY_SCENE_NAME, new DataObject(DataObject.VisibilityOptions.Public, scene_name) }
                    
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