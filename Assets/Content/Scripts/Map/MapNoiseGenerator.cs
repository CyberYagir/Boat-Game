using System;
using System.Collections.Generic;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.Map
{
    public static class MapNoiseGenerator
    {
        
        
        public static List<SaveDataObject.MapData.IslandData> GetIslandPoints(int seed, List<MapPathObject> paths, NoiseGenerator noiseGenerator)
        {
            var random = new Random(seed);

            MapPathObject mapPath = paths.GetRandomItem(random);
            var scale = mapPath.TexturePath.width;
            
            
            noiseGenerator.GetNoise(seed, scale, scale);
            
            

            List<SaveDataObject.MapData.IslandData> list = new List<SaveDataObject.MapData.IslandData>();

            for (int x = 0; x < scale; x++)
            {
                for (int y = 0; y < scale; y++)
                {
                    if (noiseGenerator.IsInRange(noiseGenerator.GeneratedNoise[x, y]))
                    {
                        Color col = mapPath.TexturePath.GetPixel(x, y);
                        if (col.a == 0)
                        {
                            int guid;
                            do
                            {
                                guid = Guid.NewGuid().GetHashCode();
                                
                            } while (list.Find(tmp => tmp.IslandSeed == guid) != null);
                            list.Add(new SaveDataObject.MapData.IslandData(new Vector2Int(x, y), guid));
                        }
                    }
                }
            }

            return list;
        }
    }
}
