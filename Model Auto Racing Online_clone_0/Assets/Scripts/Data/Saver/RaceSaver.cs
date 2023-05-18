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
        private bool is_multiplayer = false;

        private string map_name;
        public string MapName { get => map_name; private set => map_name=value; }

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
            this.MapName = map.name;

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
            this.MapName = map.name;
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
        public bool getMultiplayer() 
        {
            return is_multiplayer;
        }
        public void setMultiplayer(bool is_multiplayer = true)
        {
            this.is_multiplayer = is_multiplayer;
        }

        public void excludeTournament()
        {
            is_tournament = false;
            this.income_factor = 3;
        }
        public void setMapName() 
        {
            this.MapName = map.name;
        }

        public static RaceSaver RaceSaverFromData(RaceData rd) 
        {
            MapSaver ms = MapSaver.MapSaverFromData(rd.map);
            RaceSaver ret = new RaceSaver(ms);
            ret.lap = rd.lap;
            ret.is_race = rd.is_race;
            ret.is_tournament = rd.isTournament();
            ret.setMapName();
            ret.type = rd.type;
            ret.cost = rd.cost;
            ret.buttonPic = rd.buttonPic;
            ret.difficulty = rd.difficulty;
            ret.income_factor = rd.income_factor;
            ret.opponent = rd.opponent;
            ret.order = rd.order;

            List<Standing> s = new List<Standing>();
            foreach (RaceData.Standing rds in rd.standings) 
            {
                Standing rss = new Standing();
                rss.person = rds.person;
                rss.pos = rds.pos;
                s.Add(rss);
            }

            return ret;
        }

        public RaceData RaceDataFromSaver()
        {
            MapData md = this.map.MapDataFromSaver();
            RaceData ret = RaceData.Create(md, this.lap, this.opponent, this.is_race, this.type, this.order, this.difficulty, this.income_factor, this.cost);
            ret.includeTournament(income_factor);

            List<RaceData.Standing> s = new List<RaceData.Standing>();
            foreach (Standing rss in standings)
            {
                RaceData.Standing rds = new RaceData.Standing();
                rds.person = rss.person;
                rds.pos = rss.pos;
                s.Add(rds);
            }
            ret.standings = s;
            return ret;
        }

    }
}