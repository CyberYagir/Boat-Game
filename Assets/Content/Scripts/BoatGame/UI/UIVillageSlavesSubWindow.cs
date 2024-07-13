using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlavesSubWindow : AnimatedWindow
    {
        [SerializeField] private UIVillageSlaveItem item;
        
        
        [SerializeField] private List<UIVillageSlaveItem> spawnedItems = new List<UIVillageSlaveItem>();
        private GameDataObject gameData;
        private ResourcesService resourcesService;
        private UIVillageOptionsWindow baseWindow;
        private SaveDataObject saveData;

        private UIVillageSlavesVisualsGenerator generator;
        private List<DisplayCharacter> characters => generator.Characters;

        private bool inited = false;

        public void Init(
            GameDataObject gameData,
            ResourcesService resourcesService,
            UIVillageSlavesVisualsGenerator generator,
            UIVillageOptionsWindow window)
        {

            this.gameData = gameData;
            this.generator = generator;
            this.baseWindow = window;
            this.resourcesService = resourcesService;

            foreach (var spawned in spawnedItems)
            {
                spawned.Dispose();
                Destroy(spawned.gameObject);
            }
            spawnedItems.Clear();
            
            
            Redraw();

            inited = true;
        }

        private void OnEnable()
        {
            if (inited)
                UpdateItems();
        }


        public void Redraw()
        {
            SpawnItems();
            UpdateItems();
        }

        private void UpdateItems()
        {
            var village = baseWindow.GetVillage();
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                characters[i].Display.DeadCheck();
                spawnedItems[i].UpdateItem(village.IsHaveSlave(characters[i].Character.Uid));
            }
        }

        private void SpawnItems()
        {
            if (spawnedItems.Count == 0)
            {
                item.gameObject.SetActive(true);
                for (int i = 0; i < characters.Count; i++)
                {
                    var display = characters[i];
                    Instantiate(item, item.transform.parent)
                        .With(x => spawnedItems.Add(x))
                        .With(x => x.Init(display, resourcesService, gameData.ConfigData.MoneyItem, this));
                }

                item.gameObject.SetActive(false);
            }
        }



        public void BuySlave(DisplayCharacter displayCharacter)
        {
            if (baseWindow.BuySlave(displayCharacter.Character, displayCharacter.Cost))
            {
                UpdateItems();
            }
        }
    }
}
