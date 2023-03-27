using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Enemy List", menuName = "Enemies List")]
    public class Enemies : ScriptableObject
    {
        public List<PersonalData> enemy_list = new List<PersonalData>();
        
    }
}

    
