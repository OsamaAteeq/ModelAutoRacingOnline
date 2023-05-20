using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
        transform.position = t.position;
        transform.rotation = t.rotation;
    }
}
