using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets;
using UnityStandardAssets.Vehicles.Car;

public class carModifier : MonoBehaviour
{
    [Header("Wheels :")]
    [SerializeField] public bool _supportsWheel;
    [SerializeField] public WheelsList allSupportedWheels;
    [Header("Colors :")]
    [SerializeField] public bool _supportsColor;
    [SerializeField] public ColorsList allSupportedColors;
    [SerializeField] private Material mainMaterial;
    [SerializeField] private List<Renderer> mainRenderers;
    [SerializeField] private Material darkerMaterial;
    [SerializeField] private List<Renderer> darkerRenderers;
    [SerializeField] private float darkFactor = 4;
    [SerializeField] private Material lighterMaterial;
    [SerializeField] private List<Renderer> lighterRenderers;
    [SerializeField] private float lightFactor = 2;

    [Header("Spoilers :")]
    [SerializeField] public bool _supportsSpoilers;
    [SerializeField] private GameObject _attachedSpoiler;
    [SerializeField] public SpoilersList allSupportedSpoilers;
    [Header("Motors :")]
    [SerializeField] public bool _supportsMotors;
    [SerializeField] public MotorsList allSupportedMotors;
    [Header("Suspensions :")]
    [SerializeField] public bool _supportsSuspensions;
    [SerializeField] public SuspensionsList allSupportedSuspensions;

    [Header("Inherit References :")]
    

    private WheelCollider[] old_WheelColliders = new WheelCollider[4];
    private GameObject[] old_WheelMeshes = new GameObject[4];
    private WheelEffects[] old_WheelEffects = new WheelEffects[4];

    [SerializeField]
    private Suspension[] suspensions = new Suspension[4];

    private CarController cc;

    private int currentWheelIndex = 0;
    public int Wheel { get => currentWheelIndex; }
    private int currentColorIndex = 0;
    public int CarColor { get => currentColorIndex; }
    private int currentSuspensionIndex = 0;
    public int Suspension { get => currentSuspensionIndex; }
    private int currentSpoilerIndex = 0;
    public int Spoiler { get => currentSpoilerIndex; }
    private int currentMotorIndex = 0;
    public int Motor { get => currentMotorIndex; }

    private bool isMultiplayer;
    private void OnEnable()
    {
        cc = GetComponent<CarController>();
        old_WheelColliders = cc.m_WheelColliders;
        old_WheelMeshes = cc.m_WheelMeshes;
        old_WheelEffects = cc.m_WheelEffects;

        //changeColor(5);//FOR TESTING
        isMultiplayer = IsMultiplayer();
    }

    private bool IsMultiplayer()
    {
        try
        {
            return NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;
        }
        catch (Exception e) 
        {
            Debug.Log(e);
            return false;
        }
    }

    public void changeWheels(int i)
    {
        if (_supportsWheel)
        {
            ParticleSystem skidParticles = old_WheelColliders[0].GetComponent<WheelEffects>().skidParticles;
            Data.Wheel toAttach = allSupportedWheels.wheels[i];
            int length = old_WheelMeshes.Length;
            for (int j = 0; j < length; j++)
            {
                WheelCollider temp_collider = old_WheelColliders[j];
                GameObject temp_wheel = old_WheelMeshes[j];
                float old_distance = (old_WheelColliders[j].radius + old_WheelColliders[j].suspensionDistance) * 2 * Mathf.PI;
                Transform t = old_WheelMeshes[j].transform;

                if (j == 0)
                    old_WheelMeshes[j] = Instantiate(toAttach.front_right_wheel, t.parent.transform);
                else if (j == 1)
                    old_WheelMeshes[j] = Instantiate(toAttach.front_left_wheel, t.parent.transform);
                else if (j == 2)
                    old_WheelMeshes[j] = Instantiate(toAttach.rear_right_wheel, t.parent.transform);
                else if (j == 3)
                    old_WheelMeshes[j] = Instantiate(toAttach.rear_left_wheel, t.parent.transform);
                else
                {
                    old_WheelMeshes[j] = null;
                    Debug.LogError("Wheel Out of Bound");
                }

                if (isMultiplayer)
                {
                    old_WheelMeshes[j].GetComponent<NetworkObject>().Spawn(true);
                }

                old_WheelMeshes[j].transform.position = t.position;
                old_WheelMeshes[j].transform.rotation = t.rotation;
                //Destroy(old);
                //old.SetActive(false);

                old_WheelColliders[j] = old_WheelMeshes[j].GetComponentInChildren<WheelCollider>();
                old_WheelEffects[j] = old_WheelColliders[j].gameObject.GetComponentInChildren<WheelEffects>();

                float new_distance = (old_WheelColliders[j].radius + old_WheelColliders[j].suspensionDistance) * 2 * Mathf.PI;
                float diffrence = (new_distance - old_distance);
                if (Mathf.Abs(diffrence) < 0.0001)
                {
                    diffrence = 0;
                }
                Debug.Log("Diffrence : " + diffrence);
                old_WheelColliders[j].transform.position = new Vector3(old_WheelColliders[j].transform.position.x, old_WheelColliders[j].transform.position.y - diffrence, old_WheelColliders[j].transform.position.z);
                old_WheelColliders[j].transform.position = new Vector3(old_WheelColliders[j].transform.position.x, old_WheelColliders[j].transform.position.y + diffrence, old_WheelColliders[j].transform.position.z);

                old_WheelColliders[j].transform.SetParent(temp_collider.transform.parent);
                if (suspensions[j] != null)
                {
                    suspensions[j].wheel = old_WheelMeshes[j];
                    suspensions[j].check();
                }

                //Destroy(old_WheelColliders[j].gameObject);
                //old_WheelColliders[j].gameObject.SetActive(false);
                old_WheelColliders[j].GetComponent<WheelEffects>().skidParticles = skidParticles;
                Destroy(temp_collider.gameObject);
                Destroy(temp_wheel);

            }
            changeSuspensions(currentSuspensionIndex);
            currentWheelIndex = i;
            cc.startFunction();
        }
        else 
        {
            Debug.LogError("The vehicle : "+cc.gameObject.name+" does not support WHEELS but a script is trying to change");
        }
    }
    public void changeSpoilers(int i)
    {
        if (_supportsSpoilers)
        {
            Transform t = _attachedSpoiler.transform;

            Spoiler toAttach = allSupportedSpoilers.spoilers[i];
            BoxCollider[] old_colliders = _attachedSpoiler.GetComponentsInChildren<BoxCollider>();
            BoxCollider[] new_colliders = toAttach.spoiler.GetComponentsInChildren<BoxCollider>();
            float addToNew = 0;
            if (old_colliders.Length != 0) 
            {
                float max = old_colliders[0].size.y;
                if (old_colliders.Length > 1) 
                {
                    for (int j = 1; j < old_colliders.Length; j++) 
                    {
                        if (old_colliders[j].size.y > max) 
                        {
                            max = old_colliders[j].size.y;
                        }
                    }
                }
                addToNew -= max / 2;
            }
            if (new_colliders.Length != 0)
            {
                float max = new_colliders[0].size.y;
                if (new_colliders.Length > 1)
                {
                    for (int j = 1; j < new_colliders.Length; j++)
                    {
                        if (new_colliders[j].size.y > max)
                        {
                            max = new_colliders[j].size.y;
                        }
                    }
                }
                addToNew += max / 2;
            }
            GameObject temp = _attachedSpoiler;
            Destroy(temp);
            _attachedSpoiler = Instantiate(toAttach.spoiler, t.parent.transform);
            if (isMultiplayer)
            {
                _attachedSpoiler.GetComponent<NetworkObject>().Spawn(true);
            }
            if (toAttach.isPainted)
            {
                Material newMaterial = new Material(toAttach.paintedMaterial);
                newMaterial.color = mainMaterial.color;
                Renderer[] rs = _attachedSpoiler.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rs)
                {
                    Material[] ms = r.sharedMaterials;
                    int length = ms.Length;
                    for (int iter = 0; iter < length; iter++)
                    {
                        if (ms[iter] == toAttach.paintedMaterial)
                        {
                            ms[iter] = newMaterial;
                        }
                    }
                }
            }
            _attachedSpoiler.transform.position = new Vector3(t.position.x, t.position.y + addToNew, t.position.z);
            _attachedSpoiler.transform.rotation = t.rotation;

            currentSpoilerIndex = i;
        }
        else 
        {
            Debug.LogError("The vehicle : "+cc.gameObject.name+" does not support SPOILERS but a script is trying to change");
        }
    }
    public void changeColor(int i)
    {
        if (_supportsColor)
        {
            List<Material> mainMaterialInstances = new List<Material>();
            List<Material> darkMaterialInstances = new List<Material>();
            List<Material> lightMaterialInstances = new List<Material>();

            Color changeToColor = allSupportedColors.colors[i].color;

            Debug.Log("COLOR CHANGED TO : " + allSupportedColors.colors[i].name);
            Color changeToDark = allSupportedColors.colors[i].color;
            Color changeToLight = allSupportedColors.colors[i].color;

            float hue, saturation, value;

            foreach (Renderer renderer in mainRenderers)
            {
                Material materialInstance = new Material(renderer.material);
                renderer.material = materialInstance;
                mainMaterialInstances.Add(materialInstance);
            }
            if (darkerMaterial != null)
            {
                foreach (Renderer renderer in darkerRenderers)
                {
                    Material materialInstance = new Material(renderer.material);
                    renderer.material = materialInstance;
                    darkMaterialInstances.Add(materialInstance);
                }
                Color.RGBToHSV(changeToColor, out hue, out saturation, out value);
                changeToDark = Color.HSVToRGB(hue, saturation, value / darkFactor);
            }

            if (lighterMaterial != null)
            {
                foreach (Renderer renderer in lighterRenderers)
                {
                    Material materialInstance = new Material(renderer.material);
                    renderer.material = materialInstance;
                    lightMaterialInstances.Add(materialInstance);
                }
                Color.RGBToHSV(changeToColor, out hue, out saturation, out value);
                changeToLight = Color.HSVToRGB(hue, saturation / lightFactor, value);
            }

            if (mainMaterialInstances.Count > 0)
                foreach (Material materialInstance in mainMaterialInstances)
                    materialInstance.color = changeToColor;
            if (darkMaterialInstances.Count > 0)
                foreach (Material materialInstance in darkMaterialInstances)
                    materialInstance.color = changeToDark;
            if (lightMaterialInstances.Count > 0)
                foreach (Material materialInstance in lightMaterialInstances)
                    materialInstance.color = changeToLight;


            currentColorIndex = i;
        }
        else
        {
            Debug.LogError("The vehicle : " + cc.gameObject.name + " does not support COLORS but a script is trying to change");
        }
    }
    public void changeMotor(int i)
    {
        if (_supportsMotors)
        {
            CarController newCarController = allSupportedMotors.motors[i].carController;
            cc.m_MaximumSteerAngle = newCarController.m_MaximumSteerAngle;
            cc.m_SteerHelper = newCarController.m_SteerHelper;
            cc.m_TractionControl = newCarController.m_TractionControl;
            cc.m_FullTorqueOverAllWheels = newCarController.m_FullTorqueOverAllWheels;
            cc.m_ReverseTorque = newCarController.m_ReverseTorque;
            cc.m_Downforce = newCarController.m_Downforce;
            cc.m_SpeedType = newCarController.m_SpeedType;
            cc.m_Topspeed = newCarController.m_Topspeed;
            cc.m_RevRangeBoundary = newCarController.m_RevRangeBoundary;
            cc.m_SlipLimit = newCarController.m_SlipLimit;
            cc.m_BrakeTorque = newCarController.m_BrakeTorque;

            currentMotorIndex = i;
        }
        else
        {
            Debug.LogError("The vehicle : " + cc.gameObject.name + " does not support MOTORS but a script is trying to change");
        }
    }
    public void changeSuspensions(int i)
    {
        if (_supportsSuspensions)
        {
            WheelCollider newWheelCollider = allSupportedSuspensions.suspensions[i].wheelCollider;
            foreach (WheelCollider wc in old_WheelColliders) 
            {
                wc.wheelDampingRate = newWheelCollider.wheelDampingRate;
                wc.suspensionSpring = newWheelCollider.suspensionSpring;
                wc.suspensionDistance = newWheelCollider.suspensionDistance;
            }
            currentSuspensionIndex = i;
        }
        else
        {
            Debug.LogError("The vehicle : " + cc.gameObject.name + " does not support SUSPENSIONS but a script is trying to change");
        }
    }
}
/**/