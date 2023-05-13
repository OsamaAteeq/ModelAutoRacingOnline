using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Motor", menuName = "Motor")]
    public class MotorItem : Item
    {
        [Header("Image :")]
        public Sprite motorImage;
        public float torque;
    }
}


