//SERVER RPC : CURRENT CLIENT -----------> SERVER               FUNCTION IS RUN ON SERVER
//CLIENT RPC : SERVER ----> ALL CONNECTED CLIENTS               FUNCTION IS RUN ON ALL CONNECTED CLIENTS

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using Random = UnityEngine.Random;

using UnityStandardAssets.Vehicles.Car;
using UnityStandardAssets.Utility;
using UnityEngine.SceneManagement;
using System.Collections;
using BayatGames.SaveGameFree;
using Unity.Netcode;
using Unity.Netcode.Components;

public class MultiplayerRaceManager : NetworkBehaviour
{
    #region Variables
    [Header("Race Settings :")]
    public Data.RaceData.RaceType raceType = Data.RaceData.RaceType.Race;
    public int _winInTop = 3;
    [SerializeField]
    private int _elemenateEachLap = 1;

    public int wincount = 0;


    public int laps = 1;
    public int currentlap = 1;


    public int startcount = 0;
    public bool _startcount = false;

    public float count = 0;

    public bool startrace = false;

    [Header("Inherit UI References :")]
    public Text timerLabel;
    public Text lapsLabel;
    public Text lapscounter;
    public GameObject place;

    public Text entertostart;
    public Text finallaptext;


    public Font myfont;

    public CanvasRenderer cr;

    [Header("Inherit Scripts :")]
    public MobilePowerChange power;
    public MobileSteeringWheel steer;
    public cameraMotionScript camera;

    [Header("Race Audio :")]
    public AudioSource[] beeps;

    public AudioClip backmusic;
    public AudioClip racestart;
    public AudioClip winrace;
    public AudioClip loserace;
    public AudioClip finallap;

    [Header("Race Images :")]
    public RawImage semaphore;

    public Material sem0;
    public Material sem1;
    public Material sem2;
    public Material sem3;


    [Header("Inherit Route :")]
    public GameObject route;

    [Header("Scriptable Objects :")]
    public CarsList carList;
    public Enemies enemies;


    //Private

    private float time = 0;
    private bool finished = false;
    private int audioplayed = 0;

    private float minutes;
    private float seconds;
    private float fraction;
    private CarController[] cars;
    private List<CarController> passedCars = new List<CarController>();
    private CarMultiplayerControl player;


    private RaceSaver raceSaver;
    private VehicleSaver current_vehicle;

    private bool race;
    public int opp;

    public MultiplayerCarController serverCar;
    public MultiplayerCarController localCar;
    public List<MultiplayerCarController> playerCars = new List<MultiplayerCarController>();
    CarController[] preset_cars;
    #endregion
    //NETWORK VARIABLES


    #region Awake
    private void Awake()
    {
        if (!IsMultiplayer())
        {
            this.gameObject.SetActive(false);
        }
    }
    private bool IsMultiplayer()
    {
        return NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;
    }
    #endregion

    #region structs
    private struct MultiplayerData : INetworkSerializable
    {
        public int laps;
        public RaceData.RaceType raceType;
        public int opp;
        public MultiplayerData(int laps, RaceData.RaceType raceType, int opp)
        {
            this.laps = laps;
            this.raceType = raceType;
            this.opp = opp;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref laps);
            serializer.SerializeValue(ref raceType);
            serializer.SerializeValue(ref opp);
        }
    }
    private struct TransformData : INetworkSerializable
    {
        public Vector3 position;
        public Quaternion rotation;
        public TransformData(Vector3 position,Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
        }
    }
    private struct carData : INetworkSerializable
    {
        public int carIndex;
        public int wheelsIndex;
        public int motorsIndex;
        public int spoilersIndex;
        public int colorsIndex;
        public int suspensionsIndex;
        public carData(
        int carIndex,
        int wheelsIndex,
        int motorsIndex,
        int spoilersIndex,
        int colorsIndex,
        int suspensionsIndex)
        {
            this.carIndex = carIndex;
            this.wheelsIndex = wheelsIndex;
            this.motorsIndex = motorsIndex;
            this.spoilersIndex = spoilersIndex;
            this.colorsIndex = colorsIndex;
            this.suspensionsIndex = suspensionsIndex;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref carIndex);
            serializer.SerializeValue(ref wheelsIndex);
            serializer.SerializeValue(ref motorsIndex);
            serializer.SerializeValue(ref spoilersIndex);

            serializer.SerializeValue(ref colorsIndex);
            serializer.SerializeValue(ref suspensionsIndex);
        }
    }
    #endregion


    #region Network Variables
    private NetworkVariable<int> totalVehicles = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private NetworkVariable<MultiplayerData> _multiplayerData = new NetworkVariable<MultiplayerData>(new MultiplayerData
    {
        laps = 1,
        raceType = RaceData.RaceType.Race,
        opp = 1
    });

    private NetworkVariable<int> _loadedPlayers = new NetworkVariable<int>(0);
    private NetworkVariable<bool> loaded = new NetworkVariable<bool>(false);
    public bool IsLoaded { get => loaded.Value; }
    #endregion
    
    public override void OnNetworkSpawn()
    {
        
        Debug.Log("THIS RACE IS MULTIPLAYER");
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
            loaded.OnValueChanged += (bool previousValue, bool newValue) => 
            {
                if (newValue == true)
                {
                    int playerCarsSize = playerCars.Count;
                    for(int j = 0; j<playerCarsSize;j++)
                    {
                        int pos = Random.Range(0, (preset_cars.Length - 1));
                        int size = preset_cars.Length;
                        List<CarController> temp = new List<CarController>();
                        for (int i = 0; i < size; i++)
                        {
                            if (i != pos)
                            {
                                temp.Add(preset_cars[i]);
                            }
                        }
                        preset_cars = temp.ToArray();
                        playerCars[j].NetworkObject.TrySetParent(this.transform);
                        playerCars[j].SetTransform(preset_cars[pos].transform);

                        playerCars[j].GetComponent<NetworkTransform>().SlerpPosition = false;
                        playerCars[j].GetComponent<NetworkTransform>().SlerpPosition = true;
                        playerCars[j].GetComponent<NetworkTransform>().SlerpPosition = false;

                        VehicleSaver vehicle = SaveGame.Load<VehicleSaver>("current_vehicle");


                        //Ask clients for vehicle data


                        //playerCars[j].gameObject.transform.position = preset_cars[j].transform.position;
                        //playerCars[j].gameObject.transform.rotation = preset_cars[j].transform.rotation;
                        //send position to clientwithId
                    }
                }
            };
        }



        /*
        VehicleSaver default_vehicle = new VehicleSaver();
        default_vehicle.cost = carList.cars[0].cost;
        default_vehicle.carIndex = 0; default_vehicle.wheelsIndex = default_vehicle.motorsIndex = default_vehicle.spoilersIndex = default_vehicle.colorsIndex = default_vehicle.suspensionsIndex = 0;
        current_vehicle = SaveGame.Load<VehicleSaver>("current_vehicle", default_vehicle);
        carData car_data = new carData(current_vehicle.cost, current_vehicle.carIndex, current_vehicle.wheelsIndex, current_vehicle.motorsIndex, current_vehicle.spoilersIndex, current_vehicle.colorsIndex, current_vehicle.suspensionsIndex);
        */
        //Debug.Log("RPC ALERT : " + "TestServerRpc begged by : " + OwnerClientId);
        //TestServerRpc(car_data, new ServerRpcParams());



        /*
        if (IsServer)
        {
            joined_cars = NetworkManager.Singleton.GetComponentsInChildren<CarController>();
            Debug.Log("JOINED CARS LENGTH IS : " + joined_cars.Length);


            List<CarController> cc_list = new List<CarController>();
            List<CarController> shuffled = new List<CarController>();

            foreach (CarController c in cars)
            {
                cc_list.Add(c);
            }

            shuffled = Shuffle<CarController>(cc_list);
            int size = shuffled.Count;
            for (int i = 0; i < size; i++)
            {
                if (i < joined_cars.Length)
                {

                    joined_cars[i].transform.position = shuffled[i].transform.position;
                    joined_cars[i].transform.rotation = shuffled[i].transform.rotation;

                    WaypointCircuit wc = shuffled[i].GetComponent<WaypointProgressTracker>().circuit;
                    joined_cars[i].GetComponent<WaypointProgressTracker>().circuit = wc;
                    shuffled[i] = joined_cars[i];
                }
                else
                {
                    shuffled[i].gameObject.SetActive(false);
                    shuffled.RemoveAt(i);
                }
            }

            size = joined_cars.Length;
            Color cc_color = new Color(.2f, .2f, .2f, 1f);
            for (int i = 0; i < size; i++)
            {
                if (joined_cars[i].GetComponent<NetworkObject>().IsOwner)
                {
                    CarController car = joined_cars[i].GetComponent<CarController>();
                    GameObject _originalCar = car.gameObject;
                    WaypointCircuit wc = car.GetComponent<WaypointProgressTracker>().circuit;

                    Debug.Log("Player Found");
                    Destroy(_originalCar);
                    Transform t = _originalCar.transform.parent.transform;
                    Transform t1 = _originalCar.transform;
                    _originalCar = NetworkManager.Instantiate(carList.cars[current_vehicle.carIndex].multiplayerCar, t);
                    _originalCar.transform.position = t1.position;
                    _originalCar.transform.rotation = t1.rotation;
                    carModifier a = car.GetComponent<carModifier>();
                    cc_color = car.color;

                    if (a._supportsWheel)
                        a.changeWheels(current_vehicle.wheelsIndex);
                    if (a._supportsColor)
                    {
                        a.changeColor(current_vehicle.colorsIndex);
                        cc_color = a.allSupportedColors.colors[current_vehicle.colorsIndex].color;
                    }
                    else
                    {
                        PersonalSaver ps = SaveGame.Load<PersonalSaver>("player");
                        car.color = ps.color;
                    }
                    if (a._supportsSpoilers)
                        a.changeSpoilers(current_vehicle.spoilersIndex);
                    if (a._supportsSuspensions)
                        a.changeSuspensions(current_vehicle.suspensionsIndex);
                    if (a._supportsMotors)
                        a.changeMotor(current_vehicle.motorsIndex);


                    car.multiplayerRaceManager = this;

                    player = _originalCar.GetComponent<CarMultiplayerControl>();
                    camera.car = _originalCar.transform;
                    power.mcc2 = steer.mcc2 = _originalCar.GetComponent<MultiplayerCarController>();
                    _originalCar.GetComponent<WaypointProgressTracker>().circuit = wc;
                    _originalCar.tag = "Player";
                    player.enabled = false;

                    car.color = cc_color;
                    car.GetComponent<CarSelfRighting>().SetActive(false);
                }

                totalVehicles.Value = totalVehicles.Value+1;

            }
            player.GetComponent<CarController>().color = cc_color;
        }

        cars = joined_cars;
        

        Debug.Log("CAR" + cars.Length + ": VEHICLES : " + totalVehicles);
        if (raceType == RaceData.RaceType.Elimination)
        {
            if (laps >= totalVehicles.Value)
            {
                laps = totalVehicles.Value - 1;
            }

            int elemenation_check = laps % (totalVehicles.Value - 1);
            if (elemenation_check == 0)
            {
                _elemenateEachLap = laps / (totalVehicles.Value - 1);
            }
            else
            {
                _elemenateEachLap = (laps / (totalVehicles.Value - 1)) + 1;
            }


        }*/
    }

    private void OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        _loadedPlayers.Value = (_loadedPlayers.Value + 1);
        if (clientId == NetworkManager.LocalClientId)
        {
            Debug.Log("RPC ALERT : " + "OnLoadComplete clientId: " + clientId + " scene: " + sceneName + " mode: " + loadSceneMode);
            if (IsServer)
            {
                RaceSaver _raceSaver = SaveGame.Load<RaceSaver>("multiplayer_race");
                _multiplayerData.Value = new MultiplayerData
                {
                    laps = _raceSaver.lap,
                    raceType = _raceSaver.type,
                    opp = _raceSaver.opponent
                };
                MultiplayerCarController[] pc = FindObjectsOfType<MultiplayerCarController>();
                
            }

            _multiplayerData.OnValueChanged += (MultiplayerData previousValue, MultiplayerData newValue) =>
            {
                laps = _multiplayerData.Value.laps;
                opp = _multiplayerData.Value.opp;
                raceType = _multiplayerData.Value.raceType;
            };
        }
        else if (_loadedPlayers.Value == (_multiplayerData.Value.opp))                              //ALL CLIENTS HAVE LOADED THE SCENE
        {
            LoadClientRpc();
            loaded.Value = true;
        }
        else
        {
            Debug.Log("RPC ALERT : " + "OnLoadComplete clientId: " + clientId + " scene: " + sceneName + " mode: " + loadSceneMode);
            Debug.Log("RPC ALERT : " + _loadedPlayers.Value + " ==" + (_multiplayerData.Value.opp));
            StartCoroutine(LoadCoroutine(15));

        }


    }

    IEnumerator LoadCoroutine(int seconds)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(seconds);
        if (!IsLoaded)
        {
            loaded.Value = true;
            Debug.Log("RPC ALERT : " + "MFs Joined TOO LATE");
            LoadClientRpc();
        }
        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);

    }


    #region ServerRpc


    #endregion


    #region ClientRpc


    [ClientRpc]
    private void LoadClientRpc()
    {
        Debug.Log("RPC ALERT : " + "Found local racemanager : ");
        laps = _multiplayerData.Value.laps;
        opp = _multiplayerData.Value.opp;
        raceType = _multiplayerData.Value.raceType;

        preset_cars = GetComponentsInChildren<CarController>();
        foreach (CarController car in preset_cars)
        {
            car.gameObject.SetActive(false);
        }

        MultiplayerCarController[] pc = FindObjectsOfType<MultiplayerCarController>();
        foreach (MultiplayerCarController c in pc)
        {
            if (c.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                localCar = c;
            }
            if (c.GetComponent<NetworkObject>().IsOwnedByServer) 
            {
                serverCar = c;
            }
            playerCars.Add(c);
        }

        _multiplayerData.OnValueChanged += (MultiplayerData previousValue, MultiplayerData newValue) =>
        {
            laps = _multiplayerData.Value.laps;
            opp = _multiplayerData.Value.opp;
            raceType = _multiplayerData.Value.raceType;
        };
    }

    #endregion
    /*
    [ServerRpc]
    private void TestServerRpc(carData car_data, ServerRpcParams serverRpcParams)
    {

        Debug.Log("RPC ALERT : " + "TestServerRpc called by : " + OwnerClientId);
        int random = Random.Range(0, (cars.Length - 1));
        GameObject carToReplace = cars[random].gameObject;

        Transform t1 = carToReplace.transform;
        Transform t = carToReplace.transform.parent.transform;

        IReadOnlyDictionary<ulong, NetworkClient> a = NetworkManager.Singleton.ConnectedClients;
        NetworkClient client;
        bool found = a.TryGetValue(serverRpcParams.Receive.SenderClientId, out client);
        if (!found)
        {
            return;
        }

        GameObject client_car = client.PlayerObject.gameObject;
        client_car.transform.SetParent(t);

        client_car.GetComponent<NetworkObject>().TryRemoveParent();
        client_car.GetComponent<NetworkObject>().TrySetParent(t);
        client_car.transform.position = t1.position;
        client_car.transform.rotation = t1.rotation;

        client_car.SetActive(true);

        client_car = Instantiate(carList.cars[car_data.carIndex].multiplayerCar, t);
        client_car.GetComponent<NetworkObject>().Spawn(true);

        carModifier cm = client_car.GetComponent<carModifier>();
        cm.changeWheels(car_data.wheelsIndex);
        cm.changeMotor(car_data.motorsIndex);
        cm.changeSuspensions(car_data.suspensionsIndex);
        cm.changeSpoilers(car_data.spoilersIndex);
        cm.changeColor(car_data.colorsIndex);
        client.PlayerObject = client_car.GetComponent<NetworkObject>();

    }
    [ClientRpc]
    private void TestClientRpc(multiplayerData data)
    {

        Debug.Log("RPC ALERT : " + "TestClientRpc called by : " + OwnerClientId);
        if (!IsServer)
        {
            race = data.race;
            laps = data.laps;
            raceType = data.raceType;
            opp = data.opp;
        }
    }
    [ClientRpc]
    private void StartClientRpc()
    {

        Debug.Log("RPC ALERT : " + "StartClientRpc called by : " + OwnerClientId);
        beeps = place.GetComponents<AudioSource>();
        beeps[4].PlayOneShot(racestart);
    }
    [ServerRpc]
    private void JoinedServerRpc()
    {
        Debug.Log("RPC ALERT : " + "JoinedServerRpc called by : " + OwnerClientId);
        totalVehicles.Value = totalVehicles.Value + 1;
    }
    */
    private List<T> Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        List<T> shuffledList = new List<T>(list);
        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = shuffledList[k];
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }
        return shuffledList;
    }


    //You can use this function by calling myList.Shuffle() on any instance of a List<T> that you want to shuffle, where T is the type of elements in the list. The function will shuffle the elements of the list randomly in place.


    private void UpdateFunction()
    {
        if (race && !startrace && (power.hasChange() || steer.isWheelHeld()) && !_startcount)
        {
            startcount = startcount + 1;
            _startcount = true;
        }
        if (startcount == totalVehicles.Value)
        {
            if (count < 1f)
            {
                semaphore.enabled = true;
                semaphore.material = sem0;
            }
            else if (count < 2f)
            {
                if (audioplayed == 0)
                {
                    beeps[0].Play();
                    audioplayed++;
                }
                semaphore.material = sem1;
            }
            else if (count < 3f)
            {
                if (audioplayed == 1)
                {
                    beeps[1].Play();
                    audioplayed++;
                }
                semaphore.material = sem2;
            }
            else if (count < 4f)
            {
                if (audioplayed == 2)
                {
                    beeps[2].Play();
                    audioplayed++;
                }
                semaphore.material = sem3;
                startrace = true;

                player.enabled = true;
                player.GetComponent<CarSelfRighting>().SetActive(true);
                Debug.Log("SELFRIGHTING");

            }
            else if (count < 4.5f)
            {
                if (audioplayed == 3)
                {
                    beeps[3].loop = true;
                    beeps[3].PlayOneShot(backmusic);
                    audioplayed++;
                }
            }
            else if (count < 5.5f)
            {
                startcount = 0;
                audioplayed = 0;
                count = 0;
                semaphore.enabled = false;
            }
        }
        if (startcount == totalVehicles.Value)
        {
            count += Time.deltaTime;
        }


        if (startrace)
        {
            time += Time.deltaTime;
        }

        minutes = Mathf.Floor(time / 60);
        seconds = Mathf.Floor(time) % 60;
        fraction = Mathf.Floor(time * 100) % 100;

        if (!finished)
        {
            timerLabel.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);
            lapscounter.text = "Lap " + currentlap + " / " + laps;
        }

        if (race && !finished && !startrace && !_startcount)
        {
            entertostart.enabled = true;
        }
        else
        {
            entertostart.enabled = false;
        }
    }
    public void GoBack()
    {

        StartCoroutine(waiter());

    }
    private IEnumerator waiter()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadSceneAsync("Menu");
        //my code here after 3 seconds
    }


    public void addlap(Transform who, int lap, Color color)
    {
        if (lap == laps + 1)
        {
            crossGoal(who, color);
        }

        if (who.CompareTag("Player"))
        {
            if (lap == laps + 1)
            {
                if (wincount <= _winInTop)
                {


                    beeps[3].Stop();
                    beeps[4].PlayOneShot(winrace);
                    GoBack();

                }
                else
                {
                    beeps[3].Stop();
                    beeps[4].PlayOneShot(loserace);
                    GoBack();

                }
                finished = true;
            }
            else if (lap < laps + 1)
            {

                currentlap = lap;
                lapsLabel.text = timerLabel.text + "\n" + lapsLabel.text;
                if (lap == laps && raceType != RaceData.RaceType.Elimination)
                {
                    //final lap
                    pausemusicbriefly(2.5f);
                    beeps[4].PlayOneShot(finallap);
                    finallaptext.text = "Final Lap";
                    finallaptext.enabled = true;
                    Invoke("removemes", 2);
                }
            }
        }
        if (raceType == RaceData.RaceType.Elimination)
        {
            Debug.LogWarning("Breakpoint 1 : " + who.name);
            if (!passedCars.Contains(who.gameObject.GetComponent<CarController>()))
            {
                passedCars.Add(who.gameObject.GetComponent<CarController>());
                Debug.LogWarning("Breakpoint 2 " + (totalVehicles.Value - _elemenateEachLap) + ": " + passedCars.Count);
                int y = totalVehicles.Value;
                if (passedCars.Count == (y - _elemenateEachLap))
                {
                    foreach (CarController car in cars)
                    {
                        if (!passedCars.Contains(car) && car.gameObject.activeInHierarchy)
                        {
                            car.gameObject.SetActive(false);
                            if (car.CompareTag("Player"))
                            {
                                finished = true;
                                beeps[3].Stop();
                                beeps[4].PlayOneShot(loserace);
                                GoBack();
                            }
                            Debug.Log("Deactivated : " + car.gameObject.name);
                            totalVehicles.Value = totalVehicles.Value - 1;
                        }
                    }
                    if (player.gameObject.activeInHierarchy)
                    {
                        pausemusicbriefly(2.5f);
                        beeps[4].PlayOneShot(finallap);
                        finallaptext.text = _elemenateEachLap + " Eliminated";
                        finallaptext.enabled = true;
                        Invoke("removemes", 2);
                    }
                    passedCars.Clear();
                }
            }
            else
            {
                Debug.LogWarning("Breakpoint 3 : LAP TWICE");
                List<WaypointProgressTracker> notPassed = new List<WaypointProgressTracker>();
                GameObject toRemove = new GameObject();
                int num_to_remove = _elemenateEachLap;

                float min;
                foreach (CarController car in cars)
                {
                    if (!passedCars.Contains(car) && car.gameObject.activeInHierarchy)
                    {
                        notPassed.Add(car.GetComponent<WaypointProgressTracker>());
                    }
                }
                while (num_to_remove > 0 && notPassed.Count > 0)
                {
                    min = float.MaxValue;
                    foreach (WaypointProgressTracker loser in notPassed)
                    {
                        if (loser.GetProgress() < min)
                        {
                            min = loser.GetProgress();
                            toRemove = loser.gameObject;
                        }
                    }
                    notPassed.Remove(toRemove.GetComponent<WaypointProgressTracker>());
                    toRemove.SetActive(false);
                    if (toRemove.CompareTag("Player"))
                    {
                        finished = true;
                        beeps[3].Stop();
                        beeps[4].PlayOneShot(loserace);
                        GoBack();
                    }
                    Debug.Log("Deactivated Lapped 2 times : " + toRemove.name);
                    num_to_remove--;
                    totalVehicles.Value = totalVehicles.Value - 1;
                }
                if (player.gameObject.activeInHierarchy)
                {
                    pausemusicbriefly(2.5f);
                    beeps[4].PlayOneShot(finallap);
                    finallaptext.text = _elemenateEachLap + " Eliminated";
                    finallaptext.enabled = true;
                    Invoke("removemes", 2);
                }
                passedCars.Clear();
            }
        }

    }
    public void pausemusicbriefly(float time)
    {
        beeps[3].Pause();
        Invoke("unpausemusic", time);
    }

    public void removemes()
    {
        finallaptext.enabled = false;
    }

    public void unpausemusic()
    {
        beeps[3].UnPause();
    }

    IEnumerator WaitCoroutine(int seconds)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(seconds);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    public void crossGoal(Transform who, Color color)
    {
        Debug.Log("GOAL CROSSED BY : " + who.name + "\n COLOR : " + color + "\n PLACE : " + place);
        GameObject newGO = new GameObject("myTextGO");
        newGO.transform.SetParent(cr.transform);



        Text myText = newGO.AddComponent<Text>();
        RectTransform recttrans = newGO.GetComponent<RectTransform>();
        recttrans.sizeDelta = new Vector2(400, 100);
        Vector3 pos = new Vector3(-200, 145 - 60 * wincount, 0);
        myText.transform.localPosition = pos;

        wincount++;
        myText.font = myfont;
        myText.fontStyle = FontStyle.Bold;
        myText.fontSize = 60;
        myText.alignment = TextAnchor.UpperRight;
        myText.color = color;
        myText.text = (wincount) + ".   " + string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction); ;
        who.gameObject.SetActive(false);
    }
}