using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Spoiler", menuName = "Spoiler")]
    public class Spoiler : Item
    {
        public GameObject spoiler;
        public bool isPainted;
        public Material paintedMaterial;
    }
}


