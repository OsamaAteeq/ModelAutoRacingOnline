using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Maps List", menuName = "Map List")]
    public class MapsList : ScriptableObject
    {
        public List<MapData>maps;
    }
}


