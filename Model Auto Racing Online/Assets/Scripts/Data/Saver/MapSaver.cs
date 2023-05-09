using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Data
{
    public class MapSaver 
    {
        public string name;
        public int max_opponents;
        public int max_laps;
        [SerializeField]
        public Sprite map_image;
        public string scene_name;
        public Scene map_scene;

        public MapSaver (string scene_name)
        {
            this.scene_name = scene_name;
            this.map_scene = SceneManager.GetSceneByName(scene_name);
        }
        public static MapSaver MapSaverFromData(MapData md)
        {
            MapSaver ret = new MapSaver(md.scene_name);
            ret.name = md.name;
            ret.map_image = md.map_image;
            ret.max_laps = md.max_laps;
            ret.map_scene = md.map_scene;
            ret.max_opponents = md.max_opponents;
            return ret;
        }
        public MapData MapDataFromSaver()
        {
            MapData ret = MapData.Create(this.scene_name);
            ret.name = this.name;
            ret.map_image = this.map_image;
            ret.max_laps = this.max_laps;
            ret.map_scene = this.map_scene;
            ret.max_opponents = this.max_opponents;
            return ret;
        }
        /*
        public new string name { get => name; private set { name = value; } }
        public int max_opponents { get => max_opponents; private set { max_opponents = value; } }
        public int max_laps { get => max_laps; private set { max_laps = value; } }
        public Image map_image { get => map_image; private set { map_image = value; } }
        public Scene map_scene { get => map_scene; private set { map_scene = value; } }
        public string scene_name { get => scene_name; private set { scene_name = value; } }

        
        public MapData(string name,int max_opponents,int max_laps,Image map_image,Scene map_scene) 
        {
            this.name = name;
            this.max_opponents = max_opponents;
            this.max_laps = max_laps;
            this.map_image = map_image;
            this.map_scene = map_scene;
            this.scene_name = map_scene.name;
        }

        public MapData(string name, int max_opponents, int max_laps, Image map_image, string scene_name)
        {
            this.name = name;
            this.max_opponents = max_opponents;
            this.max_laps = max_laps;
            this.map_image = map_image;
            this.scene_name = scene_name;
            this.map_scene = SceneManager.GetSceneByName(scene_name);
        }

        public MapData(int max_opponents, int max_laps, Image map_image, Scene map_scene)
        {
            this.name = map_scene.name;
            this.max_opponents = max_opponents;
            this.max_laps = max_laps;
            this.map_image = map_image;
            this.map_scene = map_scene;
            this.scene_name = map_scene.name;
        }

        public MapData(int max_opponents, int max_laps, Image map_image, string scene_name)
        {
            this.name = scene_name;
            this.max_opponents = max_opponents;
            this.max_laps = max_laps;
            this.map_image = map_image;
            this.scene_name = scene_name;
            this.map_scene = SceneManager.GetSceneByName(scene_name);
        }
        public MapData(int max_opponents, Image map_image, Scene map_scene)
        {
            this.name = map_scene.name;
            this.max_opponents = max_opponents;
            this.max_laps = int.MaxValue;
            this.map_image = map_image;
            this.map_scene = map_scene;
            this.scene_name = map_scene.name;
        }

        public MapData(int max_opponents, Image map_image, string scene_name)
        {
            this.name = scene_name;
            this.max_opponents = max_opponents;
            this.max_laps = int.MaxValue;
            this.map_image = map_image;
            this.scene_name = scene_name;
            this.map_scene = SceneManager.GetSceneByName(scene_name);
        }

        public MapData(string name,int max_opponents,Image map_image,Scene map_scene) 
        {
            this.name = name;
            this.max_opponents = max_opponents;
            this.max_laps = int.MaxValue;
            this.map_image = map_image;
            this.map_scene = map_scene;
            this.scene_name = map_scene.name;
        }

        public MapData(string name, int max_opponents, Image map_image, string scene_name)
        {
            this.name = name;
            this.max_opponents = max_opponents;
            this.max_laps = int.MaxValue;
            this.map_image = map_image;
            this.scene_name = scene_name;
            this.map_scene = SceneManager.GetSceneByName(scene_name);
        }*/
    }
}
