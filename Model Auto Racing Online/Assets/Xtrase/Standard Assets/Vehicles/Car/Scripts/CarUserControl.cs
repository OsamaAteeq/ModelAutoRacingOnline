using System;
using Unity.IO;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        internal enum InputType
        {
            Touch,
            Keyboard
        }
        private CarController m_Car; // the car controller we want to use
        private MobileCarController mobile_Car; // the car controller we want to use
        [SerializeField] private InputType inputType = InputType.Touch;
        private float v;
        private float h;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            mobile_Car = GetComponent<MobileCarController>();

        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            if (inputType == InputType.Touch)
            {
                //float h = CrossPlatformInputManager.GetAxis("Horizontal");
                h = mobile_Car.myCarH;
                //float v = CrossPlatformInputManager.GetAxis("Vertical");
                v = mobile_Car.myCarV;
            }

            //for testing only
            if (inputType == InputType.Keyboard)
            {
                h = 0;v = 0;
                if (Input.GetKey(KeyCode.UpArrow))
                    v = 1;
                if (Input.GetKey(KeyCode.DownArrow))
                    v = -1;
                if (Input.GetKey(KeyCode.LeftArrow))
                    h = -1;
                if (Input.GetKey(KeyCode.RightArrow))
                    h = 1;
            }
            //for testing only

#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
