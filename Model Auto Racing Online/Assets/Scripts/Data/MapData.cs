using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Data
{
    [CreateAssetMenu(fileName = "new Map", menuName = "Map")]
    public class MapData : ScriptableObject
    {
        public new string name;
        public SurfaceTypes surface;
        public int max_opponents;
        public int max_laps;
        public Sprite map_image;
        public string scene_name;
        public Scene map_scene;

        private void Awake()
        {
            map_scene = SceneManager.GetSceneByName(scene_name);
        }

        public static MapData Create(string scene_name)
        {
            var data = ScriptableObject.CreateInstance<MapData>();
            data.scene_name = scene_name;
            data.map_scene = SceneManager.GetSceneByName(scene_name);
            return data;
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
