using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;
using System.Collections.Generic;

public class GarageMenu : Menu
{
    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _nextCarButton;
    [SerializeField] private Button _previousCarButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _modifyButton;

    [SerializeField] private GameObject _originalCar;
    [Header("Scriptable Objects :")]
    [SerializeField] private CarsList carlist;

    private string money;
    private VehicleSaver current_vehicle;
    private VehicleListSaver moded_vehicleList;

    private Dictionary<int, GameObject> spawned_vehicles = new Dictionary<int, GameObject>();


    override
    public void SetEnable(int value)
    {
        SaveGame.Clear();
        base.SetEnable(value);
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
        money = "" + player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        VehicleSaver default_vehicle = new VehicleSaver();
        default_vehicle.carIndex = default_vehicle.wheelsIndex = default_vehicle.motorsIndex = default_vehicle.spoilersIndex = default_vehicle.colorsIndex = default_vehicle.suspensionsIndex = 0;
        current_vehicle = SaveGame.Load<VehicleSaver>("current_vehicle",default_vehicle);
        if (!SaveGame.Exists("vehicle_list"))
        {
            VehicleListSaver default_vehicle_list = new VehicleListSaver();
            int count = 0;
            foreach (Car c in carlist.cars)
            {
                VehicleSaver vs = new VehicleSaver();
                vs.carIndex = count; vs.wheelsIndex = vs.motorsIndex = vs.spoilersIndex = vs.colorsIndex = vs.suspensionsIndex = 0;
                count++;
                default_vehicle_list.moded_vehicles.Add(vs);
            }
            moded_vehicleList = default_vehicle_list;
            SaveGame.Save<VehicleListSaver>("vehicle_list", moded_vehicleList);
        }
        else 
        {
            moded_vehicleList = SaveGame.Load<VehicleListSaver>("vehicle_list");
        }
        default_vehicle.carIndex = 0; default_vehicle.wheelsIndex = default_vehicle.motorsIndex = default_vehicle.spoilersIndex = default_vehicle.colorsIndex = default_vehicle.suspensionsIndex = 0;
        current_vehicle = SaveGame.Load<VehicleSaver>("current_vehicle", default_vehicle);
        SpawnVehicle(current_vehicle.carIndex,true);
    }

    public void HandleBackButtonPressed()
    {
        _menuManager.SwitchMenu(MenuType.Play);
        /*_menuManager.CloseMenu();
        _menuManager.SwitchMenu(MenuType.Level);
        */

        /* 
        StartCoroutine(ReloadLevelAsync(() =>
        {
            _menuManager.CloseMenu();
            _menuManager.SwitchMenu(MenuType.Level);
        }));

        */

        /*
        Console.WriteLine("Mode 2 Pressed");
        transform.GetComponent<Canvas>().overrideSorting = false;
        StartCoroutine(LevelLoaderAsync("MyLevel", () =>
        {
            _menuManager.CloseMenu();
            _menuManager.SwitchMenu(MenuType.Level);
        }));
        */
        /*
        _menuManager.CloseMenu();
        _menuManager.SwitchMenu(MenuType.Main);
        SceneManager.LoadSceneAsync("MyLevel");
        */
    }

    public void SpawnVehicle(int i, bool shouldDestroy = false) 
    {
        if (shouldDestroy)
        {
            if (spawned_vehicles.ContainsKey(i)) 
            {
                spawned_vehicles.Remove(i);
            }
                Destroy(_originalCar); 
        }
        else
            _originalCar.SetActive(false);

        if (spawned_vehicles.ContainsKey(i))
        {
            _originalCar = spawned_vehicles.GetValueOrDefault(i);
            _originalCar.SetActive(true);
            current_vehicle = moded_vehicleList.moded_vehicles[i];
        }
        else 
        {
            Transform t = _originalCar.transform.parent.transform;
            Transform t1 = _originalCar.transform;
            VehicleSaver carToSpawn;
            carToSpawn = moded_vehicleList.moded_vehicles[i];
            _originalCar = Instantiate(carlist.cars[carToSpawn.carIndex].car, t);
            current_vehicle.carIndex = i;

            _originalCar.transform.position = t1.position;
            _originalCar.transform.rotation = t1.rotation;

            MonoBehaviour[] comps = _originalCar.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in comps)
            {
                if (c.GetType() == typeof(carModifier))
                {
                    carModifier a = (carModifier)c;
                    a.changeWheels(carToSpawn.wheelsIndex);
                    a.changeColor(carToSpawn.colorsIndex);
                }
                c.enabled = false;
            }
            spawned_vehicles.Add(current_vehicle.carIndex, _originalCar);
        }

        SaveGame.Save<VehicleSaver>("current_vehicle", current_vehicle);
    }

    public void HandleNextButtonPressed()
    {
        if (current_vehicle.carIndex + 1 < carlist.cars.Count)
        {
            Debug.Log("Index : "+ (current_vehicle.carIndex + 1));

            SpawnVehicle(current_vehicle.carIndex + 1);
        }

        else
        {
            Debug.Log("Index : " + 0);

            SpawnVehicle(0);
        }
    }

    public void HandleStoreButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandlePreviousButtonPressed()
    {
        if (current_vehicle.carIndex - 1 >= 0)
        {

            Debug.Log("Index : " + (current_vehicle.carIndex - 1));
            SpawnVehicle(current_vehicle.carIndex - 1);
        }

        else
        {
            Debug.Log("Index : " + (carlist.cars.Count - 1));
            SpawnVehicle(carlist.cars.Count-1);
        }
    }
    public void HandleSelectButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }

    public void HandleModifyButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }

    #region Event Handler

    public void loadGameScene()
    {
        SceneManager.LoadScene("Game");
    }
    IEnumerator LevelLoaderAsync(string name, Action OnSceneLoaded = null)
    {
        yield return SceneManager.LoadSceneAsync(name);
        OnSceneLoaded?.Invoke();
    }
    IEnumerator ReloadLevelAsync(Action OnSceneLoaded = null)
    {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        OnSceneLoaded?.Invoke();
    }
    #endregion
}
