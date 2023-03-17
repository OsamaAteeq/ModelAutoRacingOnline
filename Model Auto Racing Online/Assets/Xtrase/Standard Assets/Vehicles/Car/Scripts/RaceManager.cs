using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    public class RaceManager : MonoBehaviour
    {
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
        private CarUserControl player;
        public GameObject route;
        private void Start()
        {
            beeps = place.GetComponents<AudioSource>();
            beeps[4].PlayOneShot(racestart);
            cars = GetComponentsInChildren<CarController>();
            foreach (CarController car in cars) 
            {
                if (car.GetComponent<CarUserControl>())
                {
                    player = car.gameObject.GetComponent<CarUserControl>();
                    player.enabled = false;
                    Debug.Log("Player Found");
                }
                else if(car.gameObject.GetComponent<CarAIControl>())
                {
                    CarAIControl aicar = car.gameObject.GetComponent<CarAIControl>();
                    Aicars.Add(aicar);
                    aicar.enabled = false;
                    Debug.Log("AI Found");
                }
            }
            
        }
        private void Update()
        {
            if (!startrace && (power.hasChange() || steer.isWheelHeld()))
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
                        }
                    }
                    player.enabled = true;
                    
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
            }

            if (!finished && !startrace && !startcount)
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
                    if (wincount < 4)
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
                    if (lap == laps)
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
            Debug.Log("GOAL CROSSED BY : " + who.name);
            GameObject newGO = new GameObject("myTextGO");
            newGO.transform.SetParent(cr.transform);

            Text myText = newGO.AddComponent<Text>();
            Vector3 pos = new Vector3(35, 145 - 20 * wincount, 0);
            myText.transform.localPosition = pos;
            wincount++;
            myText.font = myfont;
            myText.fontStyle = FontStyle.Bold;
            myText.alignment = TextAnchor.UpperRight;
            myText.color = color;
            myText.text = (wincount) + ".   " + string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction); ;
        }
    }
}