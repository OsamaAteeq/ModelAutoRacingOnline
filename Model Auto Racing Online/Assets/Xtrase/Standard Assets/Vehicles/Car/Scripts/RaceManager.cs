using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using UnityStandardAssets.Vehicles.Car;
using UnityStandardAssets.Utility;


public class RaceManager : MonoBehaviour
{

    internal enum RaceType
    {
        Elimination,
        Race,
        TimeTrial
    }
    [SerializeField]
    private RaceType raceType = RaceType.Race;
    [SerializeField]
    private float _secondsForEachLap;
    [SerializeField]
    private int _winInTop = 3;
    [SerializeField]
    private int _elemenateEachLap = 1;

    public Text timerLabel;
    public Text lapsLabel;
    public Text lapscounter; public GameObject place;

    public Text entertostart;
    public Text finallaptext;

    public MobilePowerChange power;
    public MobileSteeringWheel steer;

    public AudioSource[] beeps;

    public AudioClip backmusic;
    public AudioClip racestart;
    public AudioClip winrace;
    public AudioClip loserace;
    public AudioClip finallap;

    public RawImage semaphore;

    public Material sem0;
    public Material sem1;
    public Material sem2;
    public Material sem3;

    public int wincount = 0;

    public Font myfont;

    public CanvasRenderer cr;

    public int laps = 1;
    public int currentlap = 1;

    private float time = 0;

    public bool startcount = false;

    public float count = 0;

    public bool startrace = false;

    private bool finished = false;
    private int audioplayed = 0;

    float minutes;
    float seconds;
    float fraction;

    private CarController[] cars;

    private List<CarAIControl> Aicars = new List<CarAIControl>();
    private List<CarController> passedCars = new List<CarController>();
    private CarUserControl player;
    private int totalVehicles = 0;
    private int opp;
    private string type;
    private bool race = true;
    private float secondsForTrial;
    public GameObject route;


    private void Start()
    {
        //PlayerPref
        laps = PlayerPrefs.GetInt("lap", 2);
        opp = PlayerPrefs.GetInt("opponent", 2);
        type = PlayerPrefs.GetString("type", "Race");

        secondsForTrial = laps * _secondsForEachLap;
        if (PlayerPrefs.GetString("race", "yes") == "no") 
            { race = false; }
        else 
            { race = true; }
        switch (type) 
        {
            case "Time":
                raceType = RaceType.TimeTrial;
                break;
            case "Elim":
                raceType = RaceType.Elimination;
                break;
            case "Race":
                raceType = RaceType.Race;
                break;
            default:
                raceType = RaceType.Race;
                break;
        }
        Debug.Log("LAPS: " + laps + ": OPP : " + opp + " : Type : " + type + " : RACE : " + race);

        beeps = place.GetComponents<AudioSource>();
        beeps[4].PlayOneShot(racestart);
        cars = GetComponentsInChildren<CarController>();
        
        foreach (CarController car in cars)
        {
            totalVehicles++;
            if (car.GetComponent<CarUserControl>())
            {
                player = car.gameObject.GetComponent<CarUserControl>();
                player.enabled = false;
                Debug.Log("Player Found");
            }
            else if (car.gameObject.GetComponent<CarAIControl>())
            {
            
                CarAIControl aicar = car.gameObject.GetComponent<CarAIControl>();
                Aicars.Add(aicar);
                aicar.enabled = false;
                Debug.Log("AI Found");
            }
            car.GetComponent<CarSelfRighting>().SetActive(false);
        }
        int opp_limit;
        if (race == true && type!="Time")
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
        cars = GetComponentsInChildren<CarController>();
        Debug.Log("CAR" + cars.Length + ": VEHICLES : " + totalVehicles);
        if (raceType == RaceType.Elimination)
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
            if (raceType == RaceType.TimeTrial)
            {
                if (secondsForTrial <= ((minutes * 60) + seconds))
                {
                    finished = true;
                    beeps[3].Stop();
                    beeps[4].PlayOneShot(loserace);
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
                }
                else
                {
                    beeps[3].Stop();
                    beeps[4].PlayOneShot(loserace);
                }
                finished = true;
            }
            else if (lap < laps + 1)
            {

                currentlap = lap;
                lapsLabel.text = timerLabel.text + "\n" + lapsLabel.text;
                if (lap == laps && raceType!=RaceType.Elimination)
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
        if (raceType == RaceType.Elimination)
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