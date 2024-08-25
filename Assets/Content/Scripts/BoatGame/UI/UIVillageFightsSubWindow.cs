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

            public int Level => level;

            public string Name => name;

            public int Seed => seed;
        }

        [SerializeField] private TextAsset dungeonNames;
        [SerializeField] private List<DungeonDataHolder> dungeons = new List<DungeonDataHolder>();
        [SerializeField] private UIVillageFightsDungeonItem item;
        [SerializeField] private List<UIVillageFightsDungeonItem> items;
        private string lastID;
        private UIMessageBoxManager messageBoxManager;
        private UIVillageOptionsWindow uiVillageOptionsWindow;
        private SaveDataObject saveDataObject;

        public void Init(
            SaveDataObject.MapData.IslandData.VillageData villageData,
            GameDataObject gameDataObject,
            int level,
            UIMessageBoxManager messageBoxManager,
            UIVillageOptionsWindow uiVillageOptionsWindow,
            SaveDataObject saveDataObject
        )
        {
            this.saveDataObject = saveDataObject;
            this.uiVillageOptionsWindow = uiVillageOptionsWindow;
            this.messageBoxManager = messageBoxManager;
            if (lastID == villageData.Uid) return;

            var names = dungeonNames.LinesToList();
            var rnd = villageData.GetRandom();

            var dungeonsCount = gameDataObject.ConfigData.GetDungeonsCount(rnd);


            for (int i = 0; i < dungeonsCount; i++)
            {
                var dungeon = new DungeonData(rnd.Next(int.MinValue, int.MaxValue));
                while (dungeon.Level < level - gameDataObject.ConfigData.DungeonsLevelsOffset || dungeon.Level > level + gameDataObject.ConfigData.DungeonsLevelsOffset || dungeon.Seed == 0)
                {
                    dungeon = new DungeonData(rnd.Next(int.MinValue, int.MaxValue));
                }

                dungeons.Add(new DungeonDataHolder(dungeon.Seed, names.GetRandomItem(rnd), dungeon.Level));
            }

            DrawItems(dungeonsCount);

            lastID = villageData.Uid;
        }

        private void DrawItems(int dungeonsCount)
        {
            item.gameObject.SetActive(false);

            var count = items.Count;
            for (int i = count; i < dungeonsCount + 1; i++)
            {
                Instantiate(item, item.transform.parent)
                    .With(x => items.Add(x));
            }

            for (int i = 0; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(i < dungeonsCount);

                if (i < dungeonsCount)
                {
                    items[i].Init(dungeons[i], this, saveDataObject);
                }
            }
        }

        public void EnterDungeon(DungeonDataHolder data)
        {
            messageBoxManager.ShowMessageBox("Are you sure you want to enter the dungeon?", delegate
            {
                uiVillageOptionsWindow.EnterDungeon(data);
            });
        }
    }
}
