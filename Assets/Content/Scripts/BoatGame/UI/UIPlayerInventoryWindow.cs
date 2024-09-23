using System.Collections.Generic;
using Content.Scripts.Global;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPlayerInventoryWindow : UIStorageWindow
    {
        private SaveDataObject.PlayerInventoryData playerInventory;
        private GameDataObject gameDataObject;

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