using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPlayerInventoryWindow : UIStorageWindow
    {
        [SerializeField] private TooltipDataObject tooltipSO;
        [SerializeField] private UITooltip uiTooltip;
        private SaveDataObject.PlayerInventoryData playerInventory;
        private GameDataObject gameDataObject;

        public override void Init(IResourcesService resourcesService)
        {
            base.Init(resourcesService);
            
            uiTooltip.Init(tooltipSO);
        }

        public void SetPlayerStorage(SaveDataObject saveData, GameDataObject gameDataObject)
        {
            this.gameDataObject = gameDataObject;
            playerInventory = saveData.PlayerInventory;
        }


        protected override List<RaftStorage.StorageItem> GetStorage()
        {
            return playerInventory.GetStorage(gameDataObject);
        }

        public override bool RemoveItemFromStorage(RaftStorage.StorageItem item)
        {
            return playerInventory.RemoveItem(item);
        }

        public override void ShowWindow()
        {
            base.ShowWindow();

            Redraw();
        }
    }
}