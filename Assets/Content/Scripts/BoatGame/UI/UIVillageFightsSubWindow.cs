using System.Collections.Generic;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageFightsSubWindow : MonoBehaviour
    {
        [System.Serializable]
        public class DungeonDataHolder
        {
            [SerializeField] private int seed;
            [SerializeField] private string name;
            [SerializeField] private int level;

            public DungeonDataHolder(int seed, string name, int level)
            {
                this.seed = seed;
                this.name = name;
                this.level = level;
            }
        }

        [SerializeField] private TextAsset dungeonNames;
        [SerializeField] private List<DungeonDataHolder> dungeons = new List<DungeonDataHolder>();
        private string lastID;
        
        public void Init(SaveDataObject.MapData.IslandData.VillageData villageData, GameDataObject gameDataObject, int level)
        {
            if (lastID == villageData.Uid) return;
            
            var names = dungeonNames.LinesToList();
            var rnd = villageData.GetRandom();

            var dungeonsCount = gameDataObject.ConfigData.GetDungeonsCount(rnd);


            for (int i = 0; i < dungeonsCount; i++)
            {
                var dungeon = new DungeonData(rnd.Next(int.MinValue, int.MaxValue));
                while (dungeon.Level < level - 2 || dungeon.Level > level + 2)
                {
                    dungeon = new DungeonData(rnd.Next(int.MinValue, int.MaxValue));
                }

                dungeons.Add(new DungeonDataHolder(dungeon.Seed, names.GetRandomItem(rnd), dungeon.Level));
            }

            lastID = villageData.Uid;
        }
    }
}
