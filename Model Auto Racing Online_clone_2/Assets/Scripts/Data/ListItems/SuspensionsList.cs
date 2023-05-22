using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new SuspensionsList", menuName = "SuspensionsList")]
    public class SuspensionsList : ScriptableObject
    {
        public List<SuspensionItem> suspensions = new List<SuspensionItem>();
    }
}


