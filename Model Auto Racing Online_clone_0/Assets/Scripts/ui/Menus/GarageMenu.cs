using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using BayatGames.SaveGameFree;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

public class GarageMenu : Menu
{

    [Header("Inherit Label References :")]
    [SerializeField] private TextMeshProUGUI _nameLabel;

    [SerializeField] private TextMeshProUGUI _weightLabel;
    [SerializeField] private TextMeshProUGUI _surfaceLabel;
    [SerializeField] private TextMeshProUGUI _topspeedLabel;
    [SerializeField] private TextMeshProUGUI _scaleLabel;

    [Header("Inherit Button References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;

    [SerializeField] private Button _nextCarButton;
    [SerializeField] private Button _previousCarButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _modifyButton;

    [Header("Inherit Other References :")]
    [SerializeField] private GameObject _originalCar;
    [SerializeField] private ModificationMenu modificationMenu;


    [Header("Scriptable Objects :")]
    [SerializeField] private CarsList carlist;

    private string money;
    private int int_money;
    private VehicleSaver current_vehicle;
    private VehicleListSaver moded_vehicleList;

    private Dictionary<int, GameObject> spawned_vehicles = new Dictionary<int, GameObject>();
    private bool should_destroy = true;
    private int actuallySelected;

    private InventorySaver inventory;
    private PersonalSaver player;
    private float pos;
    private string text_modify;
    private bool buy_enabled = false;

    private void Start()
    {
        text_modify = _modifyButton.GetComponentInChildren<TextMeshProUGUI>().text;
        pos = _modifyButton.transform.position.x;
    }

    override
    public void SetEnable(int value)
    {
        Image image_new = _modifyButton.GetComponentsInChildren<Image>(true)[1];
        image_new.enabled = false;
        if (buy_enabled == true) 
        {
            
            TextMeshProUGUI previous_text = _modifyButton.GetComponentInChildren<TextMeshProUGUI>();

            previous_text.rectTransform.offsetMin = new Vector2(previous_text.rectTransform.offsetMin.x - image_new.rectTransform.rect.width, previous_text.rectTransform.offsetMin.y);
        }

        Debug.Log("Garage Enabled");
        base.SetEnable(value);
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        player = SaveGame.Load<PersonalSaver>("player", temp);
        int_money = player.cash;
        money = "" + player.cash;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        if (SaveGame.Exists("inventory"))
        {
            inventory = SaveGame.Load<InventorySaver>("inventory");
        }
        else
        {
            inventory = new InventorySaver();
        }


        if (!SaveGame.Exists("vehicle_list"))
        {
            VehicleListSaver default_vehicle_list = new VehicleListSaver();
            int count = 0;
            foreach (Car c in carlist.cars)
            {
                VehicleSaver vs = new VehicleSaver();
                vs.carIndex = count; vs.wheelsIndex = vs.motorsIndex = vs.spoilersIndex = vs.colorsIndex = vs.suspensionsIndex = 0;
                vs.cost = c.cost;
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
        int loadedLength = moded_vehicleList.moded_vehicles.Count;
        int allCarsLength = carlist.cars.Count;
        if (loadedLength < allCarsLength) 
        {
            for (int i = loadedLength; i < allCarsLength; i++) 
            {
                    VehicleSaver vs = new VehicleSaver();
                    vs.carIndex = i; vs.wheelsIndex = vs.motorsIndex = vs.spoilersIndex = vs.colorsIndex = vs.suspensionsIndex = 0;
                    moded_vehicleList.moded_vehicles.Add(vs);
            }

        SaveGame.Save<VehicleListSaver>("vehicle_list", moded_vehicleList);
        }

        VehicleSaver default_vehicle = new VehicleSaver();
        default_vehicle.cost = carlist.cars[0].cost;
        default_vehicle.carIndex = 0; default_vehicle.wheelsIndex = default_vehicle.motorsIndex = default_vehicle.spoilersIndex = default_vehicle.colorsIndex = default_vehicle.suspensionsIndex = 0;
        current_vehicle = SaveGame.Load<VehicleSaver>("current_vehicle", default_vehicle);
        
        

        actuallySelected = current_vehicle.carIndex;
        SpawnVehicle(current_vehicle.carIndex,should_destroy);
        should_destroy = false;
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
                    if(a._supportsWheel)
                        a.changeWheels(carToSpawn.wheelsIndex);
                    if(a._supportsColor)
                        a.changeColor(carToSpawn.colorsIndex);
                    if (a._supportsSpoilers)
                        a.changeSpoilers(carToSpawn.spoilersIndex);
                    if (a._supportsSuspensions)
                        a.changeSuspensions(carToSpawn.suspensionsIndex);
                    if (a._supportsMotors)
                        a.changeMotor(carToSpawn.motorsIndex);
                }
                c.enabled = false;
            }

            spawned_vehicles.Add(current_vehicle.carIndex, _originalCar);
        }

        
        _nameLabel.text = carlist.cars[i].carName;
        _surfaceLabel.text = "Surface: " + carlist.cars[i].surface;
        _scaleLabel.text = "Scale: 1:" + ((int)carlist.cars[i].scale);
        _weightLabel.text = "Weight: " + (Math.Round(_originalCar.GetComponent<Rigidbody>().mass / 1000,1).ToString("0.0")) + " kg";
        _topspeedLabel.text = "TopSpeed: " + _originalCar.GetComponent<CarController>().MaxSpeed + " mph";

        CheckSelected();
    }
    private void CheckSelected() 
    {
        if (buy_enabled == true) 
        {
            buy_enabled = false;
            Image image_new = _modifyButton.GetComponentsInChildren<Image>(true)[1];
            image_new.enabled = false;
            TextMeshProUGUI previous_text = _modifyButton.GetComponentInChildren<TextMeshProUGUI>();
            previous_text.rectTransform.offsetMin = new Vector2(previous_text.rectTransform.offsetMin.x - image_new.rectTransform.rect.width, previous_text.rectTransform.offsetMin.y);

            previous_text.text = text_modify;

        }
        if (actuallySelected == current_vehicle.carIndex)
        {
            buy_enabled = false;
            Debug.Log(actuallySelected + " = " + current_vehicle.carIndex);
            _selectButton.interactable = false;
            _selectButton.gameObject.SetActive(false);
            _modifyButton.transform.position = new Vector3(_nameLabel.transform.position.x, _modifyButton.transform.position.y, _modifyButton.transform.position.z);
        }
        else
        {
            bool isFree = carlist.cars[current_vehicle.carIndex].cost == 0;
            bool bought = false;
            foreach (ItemSaver iis in inventory.list_items) 
            {
                if (iis.GetType() == typeof(VehicleSaver))
                {
                    VehicleSaver vs = (VehicleSaver)iis;
                    if (vs.carIndex == current_vehicle.carIndex)
                    {
                        bought = true;
                        break;
                    }
                }
            }

            if (isFree || bought)
            {
                buy_enabled = false;
                if (!bought)
                {
                    inventory.list_items.Add(current_vehicle);
                    SaveGame.Save<InventorySaver>("inventory", inventory);
                }
                _modifyButton.transform.position = new Vector3(pos, _modifyButton.transform.position.y, _modifyButton.transform.position.z);
                _selectButton.gameObject.SetActive(true);
                _selectButton.interactable = true;
            }
            else 
            {
                buy_enabled = true;
                Image image_new = _modifyButton.GetComponentsInChildren<Image>(true)[1];
                image_new.enabled = true;
                TextMeshProUGUI previous_text = _modifyButton.GetComponentInChildren<TextMeshProUGUI>();

                previous_text.rectTransform.offsetMin = new Vector2(previous_text.rectTransform.offsetMin.x + image_new.rectTransform.rect.width, previous_text.rectTransform.offsetMin.y);

                if (carlist.cars[current_vehicle.carIndex].cost > int_money) 
                {
                    _modifyButton.enabled = false;
                }
                else
                {
                    _modifyButton.enabled = true;
                }

                _selectButton.interactable = false;
                _selectButton.gameObject.SetActive(false);

                _modifyButton.transform.position = new Vector3(_nameLabel.transform.position.x, _modifyButton.transform.position.y, _modifyButton.transform.position.z);
                previous_text.text = "" + carlist.cars[current_vehicle.carIndex].cost;

            }
        }
    }

    private void HandleBuyButtonPressed()
    {
        Debug.Log("CAR: "+current_vehicle.carIndex+"\nCOST : "+ carlist.cars[current_vehicle.carIndex].cost);
        int_money -= carlist.cars[current_vehicle.carIndex].cost;
        player.cash = int_money;
        SaveGame.Save<PersonalSaver>("player", player);
        money = "" + int_money;
        _storeButton.GetComponentInChildren<TextMeshProUGUI>().text = money;

        inventory.list_items.Add(current_vehicle);
        SaveGame.Save<InventorySaver>("inventory", inventory);

        actuallySelected = current_vehicle.carIndex;
        CheckSelected();
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
        SaveGame.Save<VehicleSaver>("current_vehicle", current_vehicle);
        actuallySelected = current_vehicle.carIndex;
        CheckSelected();
    }

    public void HandleModifyButtonPressed()
    {
        if (!buy_enabled)
        {
            modificationMenu.Car = _originalCar;
            SaveGame.Save<VehicleSaver>("current_vehicle", current_vehicle);
            actuallySelected = current_vehicle.carIndex;
            CheckSelected();
            should_destroy = true;
            _menuManager.SwitchMenu(MenuType.Modification);
        }
        else 
        {
            HandleBuyButtonPressed();
        }
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
