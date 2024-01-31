using System;
using System.Collections.Generic;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.Map
{
    public static class MapNoiseGenerator
    {
        private static float scale = 400f;
        private static float loops = 1f;
        private static float cutout = 0.97f;
        private static List<float> octaves = new List<float>()
        {
            0.553f, 
            0.623f, 
            0.352f, 
            0.235f, 
            0.935f, 
            0.11f, 
            0.743f, 
            0.99f, 
            1.2f, 
            0.453f, 
            0.564f, 
            0.842f,
            0.143f,
            0.2765f,
            0.3452f,
            0.4634f,
            0.5123f,
            0.6634f,
            0.8234f,
            0.9643f,
            1.1424f,
        };

        
        public static List<SaveDataObject.MapData.IslandData> GetIslandPoints(int seed, List<MapPathObject> paths)
        {
            var random = new Random(seed);
            
            MapPathObject mapPath = paths.GetRandomItem(random);
            
            var offcetX = random.Next(-1000, 1000) + 0.5125f;
            var offcetY = random.Next(-1000, 1000) + 0.6234f;

            var offcetX1 = random.Next(-1000, 1000) + 0.1125f;
            var offcetY1 = random.Next(-1000, 1000) + 0.2234f;
            

            var randomedOctaves = new List<float>(octaves);
            randomedOctaves.Shuffle(random);

            List<SaveDataObject.MapData.IslandData> list = new List<SaveDataObject.MapData.IslandData>();

            
            for (int x = 0; x < scale; x++)
            {
                for (int y = 0; y < scale; y++)
                {
                    var cordX = ((x / scale) + offcetX + offcetX1) * scale;
                    var cordY = ((y / scale) + offcetY + offcetY1) * scale;

                    var value = 0f;

                    for (int i = 0; i < loops; i++)
                    {
                        value += (Mathf.PerlinNoise(cordX * randomedOctaves[i], cordY * randomedOctaves[i]));
                    }

                    value /= loops;


                    if (value >= cutout)
                    {
                        if (mapPath.TexturePath.GetPixel(x, y).a == 0)
                        {
                            list.Add(new SaveDataObject.MapData.IslandData(new Vector2Int(x, y), seed + x + y + list.Count));
                        }
                    }
                }
            }

            return list;
        }
    }
}
