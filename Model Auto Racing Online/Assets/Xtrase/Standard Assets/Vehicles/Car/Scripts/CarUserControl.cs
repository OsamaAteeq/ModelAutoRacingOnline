using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        private MobileCarController mobile_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            mobile_Car = GetComponent<MobileCarController>();

        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float h = mobile_Car.myCarH;
            //float v = CrossPlatformInputManager.GetAxis("Vertical");
            float v = mobile_Car.myCarV;


            //float v = 1f;
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
