using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "new Race", menuName = "Race")]
    public class RaceData : ScriptableObject
    {
        public enum RaceType
        {
            Race,
            TimeTrail,
            Elimination
        }
        public enum RaceOrder
        {
            Straight
        }
        public enum RaceDifficulty
        {
            Hard
        }
        public int lap;
        public MapData map;
        public int opponent;
        public bool is_race;
        public RaceType type;
        public RaceOrder order;
        public RaceDifficulty difficulty;
        public float income_factor;
        public int cost;
        public Sprite buttonPic;
        private bool is_tournament = false;

        [System.Serializable]
        public class Standing
        {
            public PersonalSaver person;
            public int pos;
        }
        public List<Standing> standings = new List<Standing>();

        public static RaceData Create(MapData map, int lap, int opponent, bool is_race, RaceType type, RaceOrder order, RaceDifficulty difficulty, float income_factor, int cost)
        {
            var data = ScriptableObject.CreateInstance<RaceData>();
            data.lap = lap;
            data.map = map;
            data.opponent = opponent;
            data.is_race = is_race;
            data.type = type;
            data.income_factor = income_factor;
            data.cost = cost;
            data.is_tournament = false;
            data.order = order;
            data.difficulty = difficulty;

            List<Standing> s = new List<Standing>();
            data.standings = s;

            return data;

        }
        public static RaceData Create(MapData map)
        {
            var data = ScriptableObject.CreateInstance<RaceData>();
            data.lap = 0;
            data.map = map;
            data.opponent = 0;
            data.is_race = false;
            data.type = RaceType.Race;
            data.order = RaceOrder.Straight;
            data.difficulty = RaceDifficulty.Hard;
            data.income_factor = 0;
            data.cost = 0;
            data.is_tournament = false;
            return data;

        }

        /*
        public int lap { get => lap; private set => lap = value; }
        public MapData map { get => map; private set => map = value; }
        public int opponent { get => opponent; private set => opponent = value; }
        public bool is_race { get => is_race; private set => is_race = value; }
        public RaceType type { get => type; private set => type = value; }
        public float income_factor { get => income_factor; private set => income_factor = value; }
        public bool is_tournament { get => is_tournament; private set => is_tournament = value; }

        public RaceData(MapData map, int lap, int opponent,bool is_race,RaceType type,float income_factor, bool is_tournament)
        {
            this.lap = lap;
            this.map = map;
            this.opponent = opponent;
            this.is_race = is_race;
            this.type = type;
            this.income_factor = income_factor;
            this.is_tournament = is_tournament;
        }
        public RaceData(MapData map, int lap, int opponent, bool is_race, RaceType type, float income_factor)
        {
            this.lap = lap;
            this.map = map;
            this.opponent = opponent;
            this.is_race = is_race;
            this.type = type;
            this.income_factor = income_factor;
            this.is_tournament = false;
        }
        public RaceData(MapData map, int lap, int opponent, bool is_race, RaceType type)
        {
            this.lap = lap;
            this.map = map;
            this.opponent = opponent;
            this.is_race = is_race;
            this.type = type;
            this.income_factor = 3;
            this.is_tournament = false;
        }
        public RaceData(MapData map)
        {
            this.lap = int.MaxValue;
            this.map = map;
            this.opponent = 0;
            this.is_race = false;
            this.is_tournament = false;
        }
        */
        public void includeTournament(float income_factor)
        {
            is_tournament = true;
            this.income_factor = income_factor;
        }
        public void excludeTournament(float income_factor)
        {
            is_tournament = false;
            this.income_factor = income_factor;
        }
        public void excludeTournament()
        {
            is_tournament = false;
            this.income_factor = 3;
        }
    }
}