using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new SpoilersList", menuName = "SpoilersList")]
    public class SpoilersList : ScriptableObject
    {
        public List<Spoiler> spoilers = new List<Spoiler>();
    }
}


