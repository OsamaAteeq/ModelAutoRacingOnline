using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileCarController : MonoBehaviour
{
    public float myCarV = 0;
    public static MobileCarController instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetCarV(float center, float vval) 
    {
        if (vval > center) 
        {
            myCarV = 1/(vval-center);
        }

        else if (vval < center)
        {
            myCarV = -1;
        }

    }
}
