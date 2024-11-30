using System.Collections.Generic;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageFightsSubWindow : MonoBehaviour
    {
        

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

            dungeons = DungeonDataHolder.GenerateDungeons(villageData, dungeonNames, gameDataObject, level);

            
            DrawItems(dungeons.Count);

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
                uiVillageOptionsWindow.EnterDungeon(data.Seed);
            });
        }
    }
}
