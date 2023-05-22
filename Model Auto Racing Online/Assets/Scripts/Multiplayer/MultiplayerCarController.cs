using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class MultiplayerCarController : NetworkBehaviour
{
    public float myCarV = 0;
    public float myCarH = 0;

    public void SetCarV(float vval) 
    {
        myCarV = vval;
    }
    public void SetCarH(float vval)
    {
        myCarH = vval;
    }
    public void SetTransform(Transform t)
    {
        if (!IsServer) {return; }
        bool org = this.GetComponent<NetworkTransform>().Interpolate;
        this.GetComponent<NetworkTransform>().Interpolate = false;
        transform.position = t.position;
        transform.rotation = t.rotation;

        this.GetComponent<NetworkTransform>().Interpolate = org;
    }

    public override void OnDestroy()
    {
        CarController cc = GetComponent<CarController>();
        if (cc.multiplayerRaceManager != null && IsServer) 
        {
            Debug.Log("RPC ALERT: " + "DESTROYED "+name);
        }
        base.OnDestroy();
    }
}
