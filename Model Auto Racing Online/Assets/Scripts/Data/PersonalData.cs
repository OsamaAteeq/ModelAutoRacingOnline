using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Person", menuName = "Person")]
    public class PersonalData : ScriptableObject
    {
        public string id;
        public string display_name;
        public int cash;
        public Color color;
    }
}

    
