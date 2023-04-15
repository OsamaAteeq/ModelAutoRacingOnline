using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets;
public class carModifier : MonoBehaviour
{/*
    public controller controllerRef;
    //I_ for index  L_ for lists P_price
    [Header("engine")]
    public bool engineUpgrade;              // idicator 
    public int I_engineUpgrade = 0;         // current index
    public int[] P_engineUpgrade;           // prices

    [Header("spoiler")]
    public bool spoilerUpgrade;
    public int I_spoilerUpgrade = 0;
    public GameObject[] L_spoilerUpgrade;   // list of objects
    public int[] P_spoilerUpgrade;


    [Header("nitrus")]
    public bool nitrusUpgrade;
    public int I_nitrusUpgrade = 0;
    public int[] P_nitrusUpgrade;


    [Header("color")]
    public GameObject car;
    public bool colorUpgrade;
    public int I_colorUpgrade = 0;
    public Material[] L_colorUpgrade;
    public int[] P_colorUpgrade;


    [Header("wheels")]
    public bool wheelUpgrade;
    public int I_wheelUpgrade = 0;
    public GameObject[] L_wheelUpgrade;
    public int[] P_wheelUpgrade;

    [Header("handling")]
    public bool handlingUpgrade;
    public int I_handlingUpgrade = 0;
    public int[] P_handlingUpgrade;

    private void Start(){

        updateValues();
        //PlayerPrefs.SetInt("currency", 99999999);
        //controllerRef = GetComponent<controller>();
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "awakeScene") return;
        updateValues();
    }
    public void applyUpgrades(){

        if (I_engineUpgrade != PlayerPrefs.GetInt((controllerRef.carName + "engineUpgrade").ToString())) {
            PlayerPrefs.SetInt((controllerRef.carName + "engineUpgrade").ToString(), I_engineUpgrade);
            updatePrices(P_engineUpgrade,I_engineUpgrade);
        }
        else if (I_colorUpgrade != PlayerPrefs.GetInt((controllerRef.carName + "colorUpgrade").ToString())) {
            PlayerPrefs.SetInt((controllerRef.carName + "colorUpgrade").ToString(), I_colorUpgrade);
            updatePrices(P_colorUpgrade, I_colorUpgrade);
        }
        else if (I_handlingUpgrade != PlayerPrefs.GetInt((controllerRef.carName + "handlingUpgrade").ToString())) {
            PlayerPrefs.SetInt((controllerRef.carName + "handlingUpgrade").ToString(), I_handlingUpgrade);
            updatePrices(P_handlingUpgrade, I_handlingUpgrade);
        }
        else if (I_nitrusUpgrade != PlayerPrefs.GetInt((controllerRef.carName + "nitrusUpgrade").ToString())) {
            PlayerPrefs.SetInt((controllerRef.carName + "nitrusUpgrade").ToString(),I_nitrusUpgrade);
            updatePrices(P_nitrusUpgrade, I_nitrusUpgrade);
        }
        else if (I_spoilerUpgrade != PlayerPrefs.GetInt((controllerRef.carName + "spoilerUpgrade").ToString())) {
            PlayerPrefs.SetInt((controllerRef.carName + "spoilerUpgrade").ToString(),I_spoilerUpgrade);
            updatePrices(P_spoilerUpgrade,I_spoilerUpgrade);
        }
        else if (I_wheelUpgrade != PlayerPrefs.GetInt((controllerRef.carName + "wheelUpgrade").ToString())) {
            PlayerPrefs.SetInt((controllerRef.carName + "wheelUpgrade").ToString(),I_wheelUpgrade);
            updatePrices(P_wheelUpgrade, I_wheelUpgrade);
        }

        //print("applied");

    }

    public void test() {
        applyUpgrades();
        updateValues();
    }

    private void applySpoilerUpgrade(){
        if (!spoilerUpgrade) return;
        foreach (GameObject G in L_spoilerUpgrade)
        {
            G.SetActive(false);
        }

        L_spoilerUpgrade[I_spoilerUpgrade].SetActive(true);

    }    
    private void applyColorUpgrade(){
        if (!colorUpgrade) return;

        car.GetComponent<MeshRenderer>().material = L_colorUpgrade[I_colorUpgrade];
    }

    private void updateValues(){
        I_engineUpgrade = PlayerPrefs.GetInt((controllerRef.carName + "engineUpgrade").ToString());
        I_colorUpgrade = PlayerPrefs.GetInt((controllerRef.carName + "colorUpgrade").ToString());
        I_handlingUpgrade = PlayerPrefs.GetInt((controllerRef.carName + "handlingUpgrade").ToString());
        I_nitrusUpgrade = PlayerPrefs.GetInt((controllerRef.carName + "nitrusUpgrade").ToString());
        I_spoilerUpgrade = PlayerPrefs.GetInt((controllerRef.carName + "spoilerUpgrade").ToString());
        I_wheelUpgrade = PlayerPrefs.GetInt((controllerRef.carName + "wheelUpgrade").ToString());
        
        applySpoilerUpgrade();
        applyColorUpgrade();

    }

    //update all prices to 0 after bought
    private void updatePrices(int[] givenArray,int index){
        if (index == 0) return;
        for (int i = 0; i < givenArray.Length;i++ ) {
            if (i <= index) {
                givenArray[i] = 0;

            }
        }

    }*/
}
