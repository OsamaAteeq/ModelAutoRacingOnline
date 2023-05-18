using System;
using Unity.IO;
using Unity.Netcode;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarMultiplayerControl : NetworkBehaviour
    {
        internal enum InputType
        {
            Touch,
            Keyboard
        }
        private CarController m_Car; // the car controller we want to use
        private MultiplayerCarController multi_Car; // the car controller we want to use
        [SerializeField] private InputType inputType = InputType.Touch;
        private float v;
        private float h;

        private void Awake()
        {
            if (!IsOwner) return;
            // get the car controller
            m_Car = GetComponent<CarController>();
            multi_Car = GetComponent<MultiplayerCarController>();

        }


        private void FixedUpdate()
        {
            if (!IsOwner) return;
            // pass the input to the car!
            if (inputType == InputType.Touch)
            {
                //float h = CrossPlatformInputManager.GetAxis("Horizontal");
                h = multi_Car.myCarH;
                //float v = CrossPlatformInputManager.GetAxis("Vertical");
                v = multi_Car.myCarV;
            }

            //for testing only
            if (inputType == InputType.Keyboard)
            {
                h = 0; v = 0;
                if (Input.GetKey(KeyCode.W))
                    v = 1;
                if (Input.GetKey(KeyCode.S))
                    v = -1;
                if (Input.GetKey(KeyCode.A))
                    h = -1;
                if (Input.GetKey(KeyCode.D))
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
