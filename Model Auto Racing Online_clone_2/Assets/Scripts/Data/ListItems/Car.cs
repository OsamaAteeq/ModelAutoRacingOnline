using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Car", menuName = "Car")]
    public class Car : Item
    {
        public string carName;
        public Scale scale;
        public SurfaceTypes surface;
        public GameObject car;
        public GameObject multiplayerCar;
    }
}


