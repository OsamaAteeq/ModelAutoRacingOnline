using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        public int cost;
    }
}


