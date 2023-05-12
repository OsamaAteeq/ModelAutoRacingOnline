using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Upgrades List", menuName = "Upgrades List")]
    public class UpgradesList : ScriptableObject
    {
        public List<Item>upgrades;
    }
}


