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
using System.Linq;

public class ModificationMenu : Menu
{
    private struct CartItem 
    {
        public int mod;
        public int cost;
        public bool bought;
       
    }

    [Header("Inherit References :")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _storeButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _confirmButton;

    [Header(" ")]
    [SerializeField] private GridLayoutGroup _categoryContainer;
    [SerializeField] private GridLayoutGroup _modsContainer;
    [SerializeField] private TextMeshProUGUI _total;
    [SerializeField] private Sprite _blankSlate;


    [Header("Button Pictures :")]
    [SerializeField] private Sprite _wheelImage;
    [SerializeField] private Sprite _colorImage;
    [SerializeField] private Sprite _spoilerImage;
    [SerializeField] private Sprite _motorImage;
    [SerializeField] private Sprite _suspensionImage;

    [Header("Button Prefab :")]
    [SerializeField] private Button _categoryButton;
    [SerializeField] private Button _modsButton;

    [Header("Scriptable Object :")]
    [SerializeField] private UpgradesList _allUpgrades;


    private Dictionary<int, CartItem> cart = new Dictionary<int, CartItem>();

    private GameObject _originalCar;
    private Button _wheelButton;
    private Button _colorButton;
    private Button _spoilerButton;
    private Button _motorButton;
    private Button _suspensionButton;

    private InventorySaver inventory;

    private List<GameObject> modButtons = new List<GameObject>();

    public GameObject Car{ set => _originalCar = value; }

    private string money;
    private int selectedCategory = -1;
    private int selectedMod = -1;

    private carModifier _modifier;

    private void Start()
    {

    }

    override
    public void SetEnable(int value)
    {
        selectedCategory = -1;
        selectedMod = -1;
        Button[] original = _categoryContainer.GetComponentsInChildren<Button>();
        foreach (Button b in original) 
        {
            Destroy(b.gameObject);
        }

        this._modifier = _originalCar.GetComponent<carModifier>();
        int count = 0;
        base.SetEnable(value);
        PersonalSaver temp = new PersonalSaver("0", "User Name", 0, new Color(255f / 255, 189f / 255, 0));
        PersonalSaver player = SaveGame.Load<PersonalSaver>("player", temp);
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


        _originalCar.GetComponentInParent<Rotate>().isTouchRotatable = true;
        carModifier _modifier = _originalCar.GetComponentInChildren<carModifier>();
        GameObject temp_object;
        if (_modifier._supportsWheel) 
        {
            temp_object = Instantiate<GameObject>(_categoryButton.gameObject, _categoryContainer.transform);
            _wheelButton = temp_object.GetComponentInChildren<Button>();
            _wheelButton.image.sprite = _wheelImage;
            count++;
            _wheelButton.onClick.AddListener(delegate { HandleCategoryButtonPressed(0); });
            HandleCategoryButtonPressed(0);
            selectedCategory = 0;
        }
        if (_modifier._supportsColor)
        {

            temp_object = Instantiate<GameObject>(_categoryButton.gameObject, _categoryContainer.transform);
            _colorButton = temp_object.GetComponentInChildren<Button>();
            _colorButton.image.sprite = _colorImage;
            count++;

            _colorButton.onClick.AddListener(delegate { HandleCategoryButtonPressed(1); });
            if (selectedCategory == -1) 
            {
                selectedCategory = 1;
            }
        }
        if (_modifier._supportsSpoilers)
        {
            count++;
            temp_object = Instantiate<GameObject>(_categoryButton.gameObject, _categoryContainer.transform);
            _spoilerButton = temp_object.GetComponentInChildren<Button>();
            _spoilerButton.image.sprite = _spoilerImage;

            _spoilerButton.onClick.AddListener(delegate { HandleCategoryButtonPressed(2); });
            if (selectedCategory == -1)
            {
                selectedCategory = 2;
            }
        }
        if (_modifier._supportsMotors)
        {
            count++;
            temp_object = Instantiate<GameObject>(_categoryButton.gameObject, _categoryContainer.transform);
            _motorButton = temp_object.GetComponentInChildren<Button>();
            _motorButton.image.sprite = _motorImage;

            _motorButton.onClick.AddListener(delegate { HandleCategoryButtonPressed(3); });
            if (selectedCategory == -1)
            {
                selectedCategory = 3;
            }
        }
        if (_modifier._supportsSuspensions)
        {
            count++;
            temp_object = Instantiate<GameObject>(_categoryButton.gameObject, _categoryContainer.transform);
            _suspensionButton = temp_object.GetComponentInChildren<Button>();
            _suspensionButton.image.sprite = _suspensionImage;

            _suspensionButton.onClick.AddListener(delegate { HandleCategoryButtonPressed(4); });
            if (selectedCategory == -1)
            {
                selectedCategory = 4;
            }
        }
        if (selectedCategory == -1) 
        {
            HandleBackButtonPressed();
        }
        _categoryContainer.transform.LeanSetLocalPosY((_categoryContainer.cellSize.y + _categoryContainer.spacing.y) *-1* count);

    }

    public void HandleCategoryButtonPressed(int category)
    {
        if (selectedCategory != category) 
        {
            selectedCategory = category;
            foreach (GameObject g in modButtons) 
            {
                Destroy(g);
            }
            modButtons.Clear();
            switch (category)
            {
                case 4:
                    {
                        List<SuspensionItem> wl = _modifier.allSupportedSuspensions.suspensions;
                        int iter = wl.Count;
                        for (int i = 0; i < iter; i++)
                        {
                            GameObject temp = Instantiate<GameObject>(_modsButton.gameObject, _modsContainer.transform);
                            modButtons.Add(temp);
                            Button temp_btn = temp.GetComponentInChildren<Button>();
                            temp_btn.image.sprite = wl[i].suspensionImage;
                            TextMeshProUGUI temp_txt = temp.GetComponentInChildren<TextMeshProUGUI>();
                            Image[] images = temp.GetComponentsInChildren<Image>();
                            bool bought = false;

                            Image innerimage = images[2];
                            Image outerimage = images[1];
                            if (i == _modifier.Suspension)
                            {
                                temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                innerimage.enabled = false;
                                temp_txt.text = "Selected";
                                selectedMod = i;
                                Debug.Log("s " + selectedMod);
                                bought = true;
                            }
                            else
                            {
                                for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                                {
                                    if (_allUpgrades.upgrades[j] == wl[i])
                                    {
                                        for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                        {
                                            if (inventory.upgrade_index[k] == j)
                                            {
                                                bought = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if ((wl[i].cost != 0) && !bought)
                                    temp_txt.text = "" + wl[i].cost;
                                else
                                {
                                    temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                    innerimage.enabled = false;
                                    outerimage.enabled = false;
                                }
                            }
                            int i_ = i;
                            bool bought_ = bought;
                            int cost_ = wl[i].cost;
                            temp_btn.onClick.AddListener(delegate { HandleModButtonPressed(i_, bought_, cost_); });
                        }
                        break;
                    }
                case 1:
                    {
                        List<ColorItem> wl = _modifier.allSupportedColors.colors;
                        int iter = wl.Count;
                        for (int i = 0; i < iter; i++)
                        {
                            GameObject temp = Instantiate<GameObject>(_modsButton.gameObject, _modsContainer.transform);
                            modButtons.Add(temp);
                            Button temp_btn = temp.GetComponentInChildren<Button>();
                            temp_btn.image.sprite = _blankSlate;
                            temp_btn.image.color = new Color(wl[i].color.r, wl[i].color.g, wl[i].color.b,1);
                            TextMeshProUGUI temp_txt = temp.GetComponentInChildren<TextMeshProUGUI>();
                            Image[] images = temp.GetComponentsInChildren<Image>();
                            bool bought = false;

                            Image innerimage = images[2];
                            Image outerimage = images[1];
                            if (i == _modifier.CarColor)
                            {
                                temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                innerimage.enabled = false;
                                temp_txt.text = "Selected";
                                selectedMod = i;
                                Debug.Log("s " + selectedMod);
                                bought = true;
                            }
                            else
                            {
                                for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                                {
                                    if (_allUpgrades.upgrades[j] == wl[i])
                                    {
                                        for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                        {
                                            if (inventory.upgrade_index[k] == j)
                                            {
                                                bought = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if ((wl[i].cost != 0) && !bought)
                                    temp_txt.text = "" + wl[i].cost;
                                else
                                {
                                    temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                    innerimage.enabled = false;
                                    outerimage.enabled = false;
                                }
                            }
                            int i_ = i;
                            bool bought_ = bought;
                            int cost_ = wl[i].cost;
                            temp_btn.onClick.AddListener(delegate { HandleModButtonPressed(i_, bought_, cost_); });
                        }
                        break; 
                    }
                case 2:
                    {
                        List<Data.Spoiler> wl = _modifier.allSupportedSpoilers.spoilers;
                        int iter = wl.Count;
                        for (int i = 0; i < iter; i++)
                        {
                            GameObject temp = Instantiate<GameObject>(_modsButton.gameObject, _modsContainer.transform);
                            modButtons.Add(temp);
                            Button temp_btn = temp.GetComponentInChildren<Button>();
                            temp_btn.image.sprite = wl[i].spoilerImage;
                            TextMeshProUGUI temp_txt = temp.GetComponentInChildren<TextMeshProUGUI>();
                            Image[] images = temp.GetComponentsInChildren<Image>();
                            bool bought = false;

                            Image innerimage = images[2];
                            Image outerimage = images[1];
                            if (i == _modifier.Spoiler)
                            {
                                temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                innerimage.enabled = false;
                                temp_txt.text = "Selected";
                                selectedMod = i;
                                Debug.Log("s " + selectedMod);
                                bought = true;
                            }
                            else
                            {
                                for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                                {
                                    if (_allUpgrades.upgrades[j] == wl[i])
                                    {
                                        for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                        {
                                            if (inventory.upgrade_index[k] == j)
                                            {
                                                bought = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if ((wl[i].cost != 0) && !bought)
                                    temp_txt.text = "" + wl[i].cost;
                                else
                                {
                                    temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                    innerimage.enabled = false;
                                    outerimage.enabled = false;
                                }
                            }
                            int i_ = i;
                            bool bought_ = bought;
                            int cost_ = wl[i].cost;
                            temp_btn.onClick.AddListener(delegate { HandleModButtonPressed(i_, bought_, cost_); });
                        }
                        break;
                    }
                case 3:
                    {
                        List<MotorItem> wl = _modifier.allSupportedMotors.motors;
                        int iter = wl.Count;
                        for (int i = 0; i < iter; i++)
                        {
                            GameObject temp = Instantiate<GameObject>(_modsButton.gameObject, _modsContainer.transform);
                            modButtons.Add(temp);
                            Button temp_btn = temp.GetComponentInChildren<Button>();
                            temp_btn.image.sprite = wl[i].motorImage;
                            TextMeshProUGUI temp_txt = temp.GetComponentInChildren<TextMeshProUGUI>();
                            Image[] images = temp.GetComponentsInChildren<Image>();
                            bool bought = false;

                            Image innerimage = images[2];
                            Image outerimage = images[1];
                            if (i == _modifier.Motor)
                            {
                                temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                innerimage.enabled = false;
                                temp_txt.text = "Selected";
                                selectedMod = i;
                                Debug.Log("s " + selectedMod);
                                bought = true;
                            }
                            else
                            {
                                for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                                {
                                    if (_allUpgrades.upgrades[j] == wl[i])
                                    {
                                        for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                        {
                                            if (inventory.upgrade_index[k] == j)
                                            {
                                                bought = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if ((wl[i].cost != 0) && !bought)
                                    temp_txt.text = "" + wl[i].cost;
                                else
                                {
                                    temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                    innerimage.enabled = false;
                                    outerimage.enabled = false;
                                }
                            }
                            int i_ = i;
                            bool bought_ = bought;
                            int cost_ = wl[i].cost;
                            temp_btn.onClick.AddListener(delegate { HandleModButtonPressed(i_, bought_, cost_); });
                        }
                        break;
                    }
                case 0:
                default:
                    {
                        selectedCategory = 0;
                        List<Data.Wheel> wl = _modifier.allSupportedWheels.wheels;
                        int iter = wl.Count;
                        for (int i = 0; i<iter; i++) 
                        {
                            GameObject temp = Instantiate<GameObject>(_modsButton.gameObject, _modsContainer.transform);
                            modButtons.Add(temp);
                            Button temp_btn = temp.GetComponentInChildren<Button>();
                            temp_btn.image.sprite = wl[i].wheelImage;
                            TextMeshProUGUI temp_txt = temp.GetComponentInChildren<TextMeshProUGUI>();
                            Image[] images = temp.GetComponentsInChildren<Image>();
                            bool bought = false;
                            
                            Image innerimage = images[2];
                            Image outerimage = images[1];
                            if (i == _modifier.Wheel)
                            {
                                temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                innerimage.enabled = false;
                                temp_txt.text = "Selected";
                                selectedMod = i;
                                Debug.Log("s " + selectedMod);
                                bought = true;
                            }
                            else
                            {
                                for (int j = 0; j< _allUpgrades.upgrades.Count; j++)
                                {
                                    if (_allUpgrades.upgrades[j] == wl[i])
                                    {
                                        for (int k = 0; k < inventory.upgrade_index.Count; k++) 
                                        {
                                            if (inventory.upgrade_index[k] == j) 
                                            {
                                                bought = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                if ((wl[i].cost != 0) && !bought)
                                    temp_txt.text = "" + wl[i].cost;
                                else 
                                {
                                    temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - innerimage.rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
                                    innerimage.enabled = false;
                                    outerimage.enabled = false;
                                }
                            }
                            int i_ = i;
                            bool bought_ = bought;
                            int cost_ = wl[i].cost;
                            temp_btn.onClick.AddListener(delegate { HandleModButtonPressed(i_,bought_, cost_); });
                        }
                        break;
                    }
            }

            int count = modButtons.Count;
            _modsContainer.transform.LeanSetLocalPosY((_modsContainer.cellSize.y + _modsContainer.spacing.y) * -1 * count);
        }
    }

    public void HandleModButtonPressed(int item, bool bought, int cost)
    {
        if (selectedMod != item)
        {
            GameObject temp = modButtons[selectedMod];
            Button temp_btn = temp.GetComponentInChildren<Button>();
            TextMeshProUGUI temp_txt = temp.GetComponentInChildren<TextMeshProUGUI>();
            temp_txt.text = "";
            Image[] images = temp.GetComponentsInChildren<Image>(true);
            if (images[2].enabled) 
            {
                temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - images[2].rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
            }
            images[2].enabled = false;
            images[1].enabled = false;

            bool previous_bought = false;
            int previous_cost = 0;

            switch (selectedCategory)
            {
                case 4:
                    {
                        for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                        {
                            if (_allUpgrades.upgrades[j] == _modifier.allSupportedSuspensions.suspensions[selectedMod])
                            {
                                for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                {
                                    if (inventory.upgrade_index[k] == j)
                                    {
                                        previous_bought = true;
                                        break;
                                    }
                                }

                                previous_cost = _allUpgrades.upgrades[j].cost;
                                break;
                            }
                        }
                        _modifier.changeSuspensions(item);
                        break;
                    }
                case 1:
                    {
                        for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                        {
                            if (_allUpgrades.upgrades[j] == _modifier.allSupportedColors.colors[selectedMod])
                            {
                                for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                {
                                    if (inventory.upgrade_index[k] == j)
                                    {
                                        previous_bought = true;
                                        break;
                                    }
                                }
                                previous_cost = _allUpgrades.upgrades[j].cost;
                                break;
                            }

                            
                        }
                        _modifier.changeColor(item);
                        break;
                    }
                case 2:
                    {
                        for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                        {
                            if (_allUpgrades.upgrades[j] == _modifier.allSupportedSpoilers.spoilers[selectedMod])
                            {
                                for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                {
                                    if (inventory.upgrade_index[k] == j)
                                    {
                                        previous_bought = true;
                                        break;
                                    }
                                }
                                previous_cost = _allUpgrades.upgrades[j].cost;
                                break;
                            }

                        }
                        _modifier.changeSpoilers(item);
                        break;
                    }
                case 3:
                    {
                        for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                        {
                            if (_allUpgrades.upgrades[j] == _modifier.allSupportedMotors.motors[selectedMod])
                            {
                                for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                {
                                    if (inventory.upgrade_index[k] == j)
                                    {
                                        previous_bought = true;
                                        break;
                                    }
                                }
                                previous_cost = _allUpgrades.upgrades[j].cost;
                                break;
                            }

                        }
                        _modifier.changeMotor(item);
                        break;
                    }
                case 0:
                default:
                    {
                        for (int j = 0; j < _allUpgrades.upgrades.Count; j++)
                        {
                            if (_allUpgrades.upgrades[j] == _modifier.allSupportedWheels.wheels[selectedMod])
                            {
                                for (int k = 0; k < inventory.upgrade_index.Count; k++)
                                {
                                    if (inventory.upgrade_index[k] == j)
                                    {
                                        previous_bought = true;
                                        break;
                                    }
                                }
                                previous_cost = _allUpgrades.upgrades[j].cost;
                                break;
                            }

                        }
                        _modifier.changeWheels(item);
                        break;
                    }
            }
            if (!previous_bought && previous_cost != 0) 
            {
                Image[] images_new = modButtons[selectedMod].GetComponentsInChildren<Image>(true);
                TextMeshProUGUI previous_text = modButtons[selectedMod].GetComponentInChildren<TextMeshProUGUI>();
                previous_text.rectTransform.offsetMin = new Vector2(previous_text.rectTransform.offsetMin.x + images_new[2].rectTransform.rect.width, previous_text.rectTransform.offsetMin.y);
                images_new[2].enabled = true;
                images_new[1].enabled = true;
                previous_text.text = "" + previous_cost;
            }

            selectedMod = item;

            temp = modButtons[selectedMod];
            temp_btn = temp.GetComponentInChildren<Button>();
            temp_txt = temp.GetComponentInChildren<TextMeshProUGUI>();
            temp_txt.text = "Selected";
            
            images = temp.GetComponentsInChildren<Image>(true);
            if (!bought && cost != 0)
            {
                temp_txt.rectTransform.offsetMin = new Vector2(temp_txt.rectTransform.offsetMin.x - images[2].rectTransform.rect.width, temp_txt.rectTransform.offsetMin.y);
            }

            images[2].enabled = false;
            images[1].enabled = true;

            CartItem thisItem = new CartItem();
            thisItem.mod = item;
            thisItem.bought = bought;
            thisItem.cost = cost;
            if (cart.ContainsKey(selectedCategory)) 
            {
                cart.Remove(selectedCategory);
            }
            cart.Add(selectedCategory, thisItem);
            int totalCost = 0;
            List<CartItem> values = cart.Values.ToList();
            foreach (CartItem ci in values) 
            {
                totalCost += ci.cost;
            }
            _total.text = ""+totalCost;
        }
    }
        public void HandleBackButtonPressed()
    {
        _originalCar.GetComponentInParent<Rotate>().isTouchRotatable = false;
        _menuManager.SwitchMenu(MenuType.Garage);
    }

    

    public void HandleSaveButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }
    public void HandleCancelButtonPressed()
    {
        Debug.Log("NOT IMPLEMENTED YET");
    }

    public void HandleStoreButtonPressed()
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
