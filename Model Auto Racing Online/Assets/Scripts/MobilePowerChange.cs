using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MobilePowerChange : MonoBehaviour
{
    [SerializeField]
    private MobileCarController mcc;
    private Slider slider; 
    private float default_slide;
    // Start is called before the first frame update
    void Start()
    {
        slider = this.GetComponent<Slider>();
        default_slide = slider.value;
    }

    public void setDefaultValue() 
    {
        float currentValue = slider.value;
        while (currentValue > default_slide)
        { 
            slider.value = Mathf.MoveTowards(currentValue, default_slide, 0.1f * Time.deltaTime);
            currentValue = slider.value;
            changeSpeed();
        }
        slider.value = default_slide;
        changeSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value != default_slide)
        {
            changeSpeed();
        }
    }

    private void changeSpeed() 
    {
        float change = slider.value - default_slide; //2 - 0  = 2 with max 6
        float actual_xtreme;
        float dec_change;

        if (change > 0)
        {
            actual_xtreme = slider.maxValue - default_slide; //6
        }
        else
        {
            actual_xtreme = -1*(slider.minValue + default_slide); //4   
        }

        dec_change = change / actual_xtreme;


        mcc.SetCarV(dec_change);
    }
}


