using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageStorageSubWindow : UIStorageWindow
    {
        [SerializeField] private TMP_Text headerText;

        private SlaveDataCalculator slaveDataCalculator;
        

        public void OpenSlaveStorage(SlaveDataCalculator slaveDataCalculator)
        {
            this.slaveDataCalculator = slaveDataCalculator;
            
            headerText.text = $"{slaveDataCalculator.CharacterData.Character.Name} storage";

            ShowWindow();
            Redraw();
        }

        public override bool RemoveItemFromStorage(RaftStorage.StorageItem item)
        {
            return slaveDataCalculator.RemoveItem(item);
        }

        protected override List<RaftStorage.StorageItem> GetStorage()
        {
            return slaveDataCalculator.GetStorage();
        }

        public void ClearButton()
        {
            slaveDataCalculator.ClearStorage();
            CloseWindow();
        }
    }
}
