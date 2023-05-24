using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace Data
{
    [CreateAssetMenu(fileName = "new Motor", menuName = "Motor")]
    public class MotorItem : Item
    {
        public CarController carController;
        [Header("Image :")]
        public Sprite motorImage;
        
    }
}


