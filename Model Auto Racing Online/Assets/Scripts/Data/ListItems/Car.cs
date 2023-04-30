using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public enum Scale
    {
        [InspectorName("1:6")]
        Six = 6,
        [InspectorName("1:8")]
        Eight = 8,
        [InspectorName("1:10")]
        Ten = 10
    }

    [CreateAssetMenu(fileName = "new Car", menuName = "Car")]
    public class Car : Item
    {
        public string carName;
        public Scale scale;
        public SurfaceTypes surface;
        public GameObject car;
    }
}


