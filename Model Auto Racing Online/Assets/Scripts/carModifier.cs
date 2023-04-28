using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets;
using UnityStandardAssets.Vehicles.Car;

public class carModifier : MonoBehaviour
{
    [Header("Wheels :")]
    [SerializeField] public bool _supportsWheel;
    [SerializeField] private WheelsList allSupportedWheels;
    [Header("Colors :")]
    [SerializeField] public bool _supportsColor;
    [SerializeField] private ColorsList allSupportedColors;
    [SerializeField] private Material mainMaterial;
    [SerializeField] private Material darkerMaterial;
    [SerializeField] private float darkFactor = 4;
    [SerializeField] private float lightFactor = 2;
    [SerializeField] private Material lighterMaterial;

    [Header("Spoilers :")]
    [SerializeField] public bool _supportsSpoilers;
    [SerializeField] private SpoilersList allSupportedSpoilers;
    [Header("Motors :")]
    [SerializeField] public bool _supportsMotors;
    [SerializeField] private MotorsList allSupportedMotors;
    [Header("Suspensions :")]
    [SerializeField] public bool _supportsSuspensions;
    [SerializeField] private SuspensionsList allSupportedSuspensions;

    [Header("Inherit References :")]
    

    private WheelCollider[] old_WheelColliders = new WheelCollider[4];
    private GameObject[] old_WheelMeshes = new GameObject[4];
    private WheelEffects[] old_WheelEffects = new WheelEffects[4];

    [SerializeField]
    private Suspension[] suspensions = new Suspension[4];

    private CarController cc;

    private void OnEnable()
    {
        cc = GetComponent<CarController>();
        old_WheelColliders = cc.m_WheelColliders;
        old_WheelMeshes = cc.m_WheelMeshes;
        old_WheelEffects = cc.m_WheelEffects;
    }

    public void changeWheels(int i)
    {
        if (_supportsWheel)
        {
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

                Destroy(temp_collider.gameObject);
                Destroy(temp_wheel);

            }
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
            Color changeToColor = allSupportedColors.colors[i].color;
            mainMaterial.color = changeToColor;
            float hue, saturation, value;
            if (darkerMaterial != null) 
            {
                Color.RGBToHSV(changeToColor, out hue, out saturation, out value);
                changeToColor = Color.HSVToRGB(hue, saturation, value/darkFactor);
                darkerMaterial.color = changeToColor;
            }
            if (lighterMaterial != null)
            {
                Color.RGBToHSV(changeToColor, out hue, out saturation, out value);
                changeToColor = Color.HSVToRGB(hue, saturation/lightFactor, value);
                lighterMaterial.color = changeToColor;
            }
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

        }
        else
        {
            Debug.LogError("The vehicle : " + cc.gameObject.name + " does not support SUSPENSIONS but a script is trying to change");
        }
    }
}
/**/