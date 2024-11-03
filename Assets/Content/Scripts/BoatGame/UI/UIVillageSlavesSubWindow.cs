using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData.SlaveData.TransferData;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageSlavesSubWindow : AnimatedWindow
    {
        [SerializeField] private UIVillageSlaveItem item;
        
        
        [SerializeField] private List<UIVillageSlaveItem> spawnedItems = new List<UIVillageSlaveItem>();
        [SerializeField] private GameObject manageIndicator;
        private GameDataObject gameData;
        private IResourcesService resourcesService;
        private UIVillageOptionsWindow baseWindow;
        private SaveDataObject saveData;

        private UIVillageSlavesGenerator generator;
        private List<DisplayCharacter> SlavesVisuals => generator.SlavesVisuals;
        private List<SlaveCreatedCharacterInfo> SlavesInfo => generator.SlavesInfos;

        private bool inited = false;

        public void Init(
            GameDataObject gameData,
            IResourcesService resourcesService,
            UIVillageSlavesGenerator generator,
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
                SlavesVisuals[i].Display.DeadCheck();
                var isTransferedFromIsland = false;
                var isBuyed = village.IsHaveSlave(SlavesInfo[i].Character.Uid);
                if (isBuyed)
                {
                    isTransferedFromIsland = SlavesInfo[i].SlaveData.TransferInfo.TransferState == ETransferState.SendFromIsland;

                    if (SlavesInfo[i].SlaveData.TransferInfo.TransferState is ETransferState.NewOnIsland or ETransferState.Hired)
                    {
                        spawnedItems[i].gameObject.SetActive(false);
                    }
                }
                
                spawnedItems[i].UpdateItem(isBuyed || isTransferedFromIsland);
            }
        }

        private void SpawnItems()
        {
            if (spawnedItems.Count == 0)
            {
                item.gameObject.SetActive(true);
                for (int i = 0; i < SlavesVisuals.Count; i++)
                {
                    var display = SlavesVisuals[i];
                    var info = SlavesInfo[i];
                    Instantiate(item, item.transform.parent)
                        .With(x => spawnedItems.Add(x))
                        .With(x => x.Init(display, info, resourcesService, gameData.ConfigData.MoneyItem, this));
                }

                item.gameObject.SetActive(false);
            }
        }



        public void BuySlave(SlaveCreatedCharacterInfo info)
        {
            if (baseWindow.BuySlave(info))
            {
                if (manageIndicator != null)
                {
                    manageIndicator.gameObject.SetActive(true);
                }
                UpdateItems();
            }
        }
    }
}
