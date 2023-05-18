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

public class RaceManager : MonoBehaviour
{
    [Header("Race Settings :")]
    [SerializeField]
    private Data.RaceData.RaceType raceType = Data.RaceData.RaceType.Race;
    [SerializeField]
    private float _secondsForEachLap;
    [SerializeField]
    private int _winInTop = 3;
    [SerializeField]
    private float _awardFactor = 3;
    [SerializeField]
    private int _elemenateEachLap = 1;

    public int wincount = 0;


    public int laps = 1;
    public int currentlap = 1;


    public bool startcount = false;

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
    private List<CarAIControl> Aicars = new List<CarAIControl>();
    private List<CarController> passedCars = new List<CarController>();
    private CarUserControl player;
    private int totalVehicles = 0;
    
    private float secondsForTrial;

    private RaceSaver raceSaver;
    private TournamentSaver tournamentSaver;
    private VehicleSaver current_vehicle;

    private bool race;
    private int opp;
    private List<PersonalSaver> selected_enemies;

    private void Start()
    {
        raceSaver = SaveGame.Load<RaceSaver>("current_race");
        if (SaveGame.Exists("current_tournament")) 
        {
            tournamentSaver = SaveGame.Load<TournamentSaver>("current_tournament");
        }

        VehicleSaver default_vehicle = new VehicleSaver();
        default_vehicle.cost = carList.cars[0].cost;
        default_vehicle.carIndex = 0; default_vehicle.wheelsIndex = default_vehicle.motorsIndex = default_vehicle.spoilersIndex = default_vehicle.colorsIndex = default_vehicle.suspensionsIndex = 0;
        current_vehicle = SaveGame.Load<VehicleSaver>("current_vehicle", default_vehicle);

        laps = raceSaver.lap;
        _awardFactor = raceSaver.income_factor;
        race = raceSaver.is_race;
        raceType = raceSaver.type;
        secondsForTrial = laps * _secondsForEachLap;
        opp = raceSaver.opponent;
        
        Debug.Log("LAPS: " + laps + ": OPP : " + opp + " : Type : " + raceType + " : RACE : " + race);

        beeps = place.GetComponents<AudioSource>();
        beeps[4].PlayOneShot(racestart);
        cars = GetComponentsInChildren<CarController>();
        int rand_player_index = (int) Math.Round((double)Random.Range(0, (cars.Length - 1)));

        int size = cars.Length;
        Color cc_color = new Color(.2f,.2f,.2f,1f);
        for (int i = 0; i<size;i++)
        {
            CarController car = cars[i].GetComponent<CarController>();
            if (totalVehicles == rand_player_index)
            {
                WaypointCircuit wc = car.GetComponent<WaypointProgressTracker>().circuit;

                GameObject _originalCar = car.gameObject;
                Debug.Log("Player Found");
                Destroy(_originalCar);
                Transform t = _originalCar.transform.parent.transform;
                Transform t1 = _originalCar.transform;
                _originalCar = Instantiate(carList.cars[current_vehicle.carIndex].car, t);
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


                car.tim = this;
                _originalCar.GetComponent<CarAIControl>().enabled = false;
                player = _originalCar.GetComponent<CarUserControl>();
                camera.car = _originalCar.transform;
                power.mcc = steer.mcc = _originalCar.GetComponent<MobileCarController>();
                _originalCar.GetComponent<WaypointProgressTracker>().circuit = wc;
                _originalCar.tag = "Player";
                player.enabled = false;

                car.color = cc_color;
            }
            else 
            {
                //NEED TO SPAWN RANDOM ENEMY VEHICLES
                car.gameObject.GetComponent<CarUserControl>().enabled = false;
                car.gameObject.GetComponent<MobileCarController>().enabled = false;
                CarAIControl aicar = car.gameObject.GetComponent<CarAIControl>();
                Aicars.Add(aicar);
                aicar.enabled = false;
                Debug.Log("AI Found");
            }
            
            totalVehicles++;
            car.GetComponent<CarSelfRighting>().SetActive(false);
        }
        player.GetComponent<CarController>().color = cc_color;
        int opp_limit;
        if (race == true && raceType != RaceData.RaceType.TimeTrail)
        {
            opp_limit = Aicars.Count - opp;
        }
        else
        {
            opp_limit = Aicars.Count;
        }
        Debug.Log("OPP_LIMIT = " + opp_limit);
        while (opp_limit > 0 && Aicars.Count>0) 
        {
            Debug.Log("REMOVED : "+Aicars.Count);
            int r = Random.Range(0, Aicars.Count - 1);
            Aicars[r].gameObject.SetActive(false);
            Aicars.RemoveAt(r);
            opp_limit--;
            totalVehicles--;
        }

        List<PersonalSaver> shuffled_enemies = Shuffle(enemies.enemy_list);
        selected_enemies = shuffled_enemies.GetRange(0, Aicars.Count);
        Color playerColor = player.GetComponent<carModifier>().allSupportedColors.colors[current_vehicle.colorsIndex].color;
        int iter = 0;
        foreach (CarAIControl cac in Aicars) 
        {
            if (selected_enemies[iter].color == playerColor) 
            {
                selected_enemies[iter] = shuffled_enemies[selected_enemies.Count];
            }
            CarController cc = cac.GetComponent<CarController>();
            cc.color = selected_enemies[iter].color;
            carModifier cm = cac.GetComponent<carModifier>();
            if (cm._supportsColor)
            {
                int length = cm.allSupportedColors.colors.Count;
                int colorindex = Random.Range(0, length - 1);
                for (int i = 0; i < length; i++)
                {
                    if (cm.allSupportedColors.colors[i].color == cc.color)
                    {
                        colorindex = i;
                        break;
                    }
                }
                Debug.Log(selected_enemies[iter].display_name);
                cm.changeColor(colorindex);
            }
            //      NEEDS RANDOM UPGRADES ON ENEMIES         //Need to be randomized   could be done at spawn
            if (cm._supportsWheel)
            { cm.changeWheels(0); }
            if (cm._supportsSpoilers)
                cm.changeSpoilers(0);
            if (cm._supportsSuspensions)
                cm.changeSuspensions(0);
            if (cm._supportsMotors)
                cm.changeMotor(0);
            
            iter++;
        }

        cars = GetComponentsInChildren<CarController>();

        Debug.Log("CAR" + cars.Length + ": VEHICLES : " + totalVehicles);
        if (raceType == RaceData.RaceType.Elimination)
        {
            int elemenation_check = laps % (totalVehicles-1);
            if (elemenation_check == 0)
            {
                _elemenateEachLap = laps / (totalVehicles - 1);
            }
            else
            {
                _elemenateEachLap = (laps / (totalVehicles-1)) + 1;
            }
            
        }
    }
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


    private void Update()
    {
        if (race&&!startrace && (power.hasChange() || steer.isWheelHeld()))
        {
            startcount = true;
        }
        if (startcount)
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
                if (Aicars.Count > 0)
                {
                    foreach (CarAIControl car in Aicars)
                    {
                        car.enabled = true;
                        car.GetComponent<CarSelfRighting>().SetActive(true);
                    }
                }
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
                startcount = false;
                audioplayed = 0;
                count = 0;
                semaphore.enabled = false;
            }
        }
        if (startcount)
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
            if (raceType == RaceData.RaceType.TimeTrail)
            {
                if (secondsForTrial <= ((minutes * 60) + seconds))
                {
                    finished = true;
                    beeps[3].Stop();
                    beeps[4].PlayOneShot(loserace);
                    GoBack();
                }
            }
        }

        if (race && !finished && !startrace && !startcount)
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
                    PersonalSaver ps = SaveGame.Load<PersonalSaver>("player");
                    int money = ps.cash;
                    money = money + (int) Math.Round(((opp + lap) * _awardFactor));
                    ps.cash = money;
                    SaveGame.Save<PersonalSaver>("player", ps);

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
                if (lap == laps && raceType!=RaceData.RaceType.Elimination)
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
            Debug.LogWarning("Breakpoint 1 : "+who.name);
            if (!passedCars.Contains(who.gameObject.GetComponent<CarController>()))
            {
                passedCars.Add(who.gameObject.GetComponent<CarController>());
                Debug.LogWarning("Breakpoint 2 " + (totalVehicles - _elemenateEachLap) + ": " + passedCars.Count);
                int y = totalVehicles;
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
                            Debug.Log("Deactivated : "+car.gameObject.name);
                            totalVehicles--;
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
                    totalVehicles--;
                }
                if (player.gameObject.activeInHierarchy)
                {
                    pausemusicbriefly(2.5f);
                    beeps[4].PlayOneShot(finallap);
                    finallaptext.text = _elemenateEachLap+" Eliminated";
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