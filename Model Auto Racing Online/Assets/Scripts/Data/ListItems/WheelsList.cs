using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new WheelsList", menuName = "WheelsList")]
    public class WheelsList : ScriptableObject
    {
        public List<Wheel> wheels = new List<Wheel>();
    }
}


