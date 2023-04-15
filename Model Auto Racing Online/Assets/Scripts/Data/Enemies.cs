using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Enemy List", menuName = "Enemies List")]
    public class Enemies : ScriptableObject
    {
        [SerializeField]
        public List<PersonalSaver> enemy_list = new List<PersonalSaver>();
        
    }
}

    
