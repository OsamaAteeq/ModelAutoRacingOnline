
using UnityEngine;
namespace Data
{
    public class PersonalSaver
    {
        public string id;
        public string display_name;
        public int cash;
        public Color color;

        public PersonalSaver(string id, string display_name, int cash, Color color)
        {
            this.id = id;
            this.display_name = display_name;
            this.cash = cash;
            this.color = color;
        }
    }
}

    
