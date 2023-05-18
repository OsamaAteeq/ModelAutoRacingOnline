using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Car List", menuName = "Car List")]
    public class CarsList : ScriptableObject
    {
        public List<Car>cars;
    }
}


