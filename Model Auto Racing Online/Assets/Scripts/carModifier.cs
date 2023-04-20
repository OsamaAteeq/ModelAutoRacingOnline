using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets;
using UnityStandardAssets.Vehicles.Car;

public class carModifier : MonoBehaviour
{
    [SerializeField]
    private WheelsList all_wheels;

    private WheelCollider[] old_WheelColliders = new WheelCollider[4];
    private GameObject[] old_WheelMeshes = new GameObject[4];

    private WheelCollider[] new_WheelColliders = new WheelCollider[4];
    private GameObject[] new_WheelMeshes = new GameObject[4];

    private CarController cc;

    private void OnEnable()
    {
        cc = GetComponent<CarController>();
        old_WheelColliders = cc.m_WheelColliders;
        old_WheelMeshes = cc.m_WheelMeshes;
    }

    public void changeWheels(int i) 
    {
        int count = 0;
        Data.Wheel toAttach = all_wheels.wheels[i];
        foreach (GameObject old in old_WheelMeshes) 
        {
            
            Transform t = old.transform;
            GameObject newWheel;
            if (count == 0)
                newWheel = Instantiate(toAttach.front_right_wheel, t.parent.transform);
            else if (count == 1)
                newWheel = Instantiate(toAttach.front_left_wheel, t.parent.transform);
            else if (count == 2)
                newWheel = Instantiate(toAttach.rear_right_wheel, t.parent.transform);
            else if (count == 3)
                newWheel = Instantiate(toAttach.rear_left_wheel, t.parent.transform);
            else
            {
                newWheel = null;
                Debug.LogError("Wheel Out of Bound");
            }

            newWheel.transform.position = t.position;
            newWheel.transform.rotation = t.rotation;
            old.SetActive(false);

            WheelCollider newCollider = newWheel.GetComponentInChildren<WheelCollider>();
            newCollider.transform.SetParent(old_WheelColliders[count].transform.parent);
            old_WheelColliders[count].gameObject.SetActive(false);

            new_WheelMeshes[count] = newWheel;
            new_WheelColliders[count] = newCollider;

            count++;
        }
    }
}
/**/