using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Inventory", menuName = "Inventory")]
    public class Inventory : ScriptableObject
    {
        public List<Item> list_items = new List<Item>();

    }
}

    
