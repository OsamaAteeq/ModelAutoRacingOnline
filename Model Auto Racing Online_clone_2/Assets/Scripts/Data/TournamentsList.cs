using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Tournament List", menuName = "Tournament List")]
    public class TournamentsList : ScriptableObject
    {
        public List<TournamentData> tournaments;
    }
}


