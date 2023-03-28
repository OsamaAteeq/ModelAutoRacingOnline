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

        public static PersonalData Create(string id, string display_name, int cash, Color color)
        {
            var data = ScriptableObject.CreateInstance<PersonalData>();
            data.id = id;
            data.display_name = display_name;
            data.cash = cash;
            data.color = color;
            return data;
        }
    }
}

    
