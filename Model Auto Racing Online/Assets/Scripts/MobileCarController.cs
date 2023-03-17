using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileCarController : MonoBehaviour
{
    public float myCarV = 0;
    public float myCarH = 0;

    // Start is called before the first frame update
    void Start()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
    }
    public void SetCarV(float vval) 
    {
        myCarV = vval;
    }
    public void SetCarH(float vval)
    {
        myCarH = vval;
    }
}
