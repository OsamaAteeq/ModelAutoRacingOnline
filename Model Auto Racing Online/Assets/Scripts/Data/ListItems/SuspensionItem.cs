using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Suspension", menuName = "Suspension")]
    public class SuspensionItem : Item
    {
        public WheelCollider wheelCollider; 
    }
}


