using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public class RaceSaver
    {
        public int lap;
        public MapSaver map;
        public int opponent;
        public bool is_race;
        public RaceData.RaceType type;
        public RaceData.RaceOrder order;
        public RaceData.RaceDifficulty difficulty;
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

        public RaceSaver (MapSaver map, int lap, int opponent, bool is_race, RaceData.RaceType type, RaceData.RaceOrder order, RaceData.RaceDifficulty difficulty, float income_factor, int cost)
        {
            this.lap = lap;
            this.map = map;
            this.opponent = opponent;
            this.is_race = is_race;
            this.type = type;
            this.income_factor = income_factor;
            this.cost = cost;
            this.is_tournament = false;
            this.order = order;
            this.difficulty = difficulty;

            List<Standing> s = new List<Standing>();
            this.standings = s;

        }

        public RaceSaver (MapSaver map)
        {
            this.lap = 0;
            this.map = map;
            this.opponent = 0;
            this.is_race = false;
            this.type = RaceData.RaceType.Race;
            this.order = RaceData.RaceOrder.Straight;
            this.difficulty = RaceData.RaceDifficulty.Hard;
            this.income_factor = 0;
            this.cost = 0;
            this.is_tournament = false;

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