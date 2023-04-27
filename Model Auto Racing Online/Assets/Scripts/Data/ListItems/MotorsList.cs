using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new MotorsList", menuName = "MotorsList")]
    public class MotorsList : ScriptableObject
    {
        public List<MotorItem> spoilers = new List<MotorItem>();
    }
}


