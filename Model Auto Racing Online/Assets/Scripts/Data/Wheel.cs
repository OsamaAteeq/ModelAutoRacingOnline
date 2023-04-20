using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Wheel", menuName = "Wheel")]
    public class Wheel : Item
    {
        [Header("Front :")]
        public GameObject front_right_wheel;
        public GameObject front_left_wheel;

        [Header("Rear :")]
        public GameObject rear_right_wheel;
        public GameObject rear_left_wheel;
    }
}


