using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new ColorsList", menuName = "ColorsList")]
    public class ColorsList : ScriptableObject
    {
        public List<ColorItem> colors = new List<ColorItem>();
    }
}


