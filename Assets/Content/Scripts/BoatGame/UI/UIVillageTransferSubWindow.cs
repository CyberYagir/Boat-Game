using System.Collections.Generic;
using Content.Scripts.Global;
using UnityEngine;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData.SlaveData.TransferData;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageTransferSubWindow : AnimatedWindow
    {
        [SerializeField] private UIVillageTransferItem item;

        private List<UIVillageTransferItem> items = new List<UIVillageTransferItem>();
        private SlaveDataCalculator slaveDataCalculator;
        private SaveDataObject saveData;
        private VillageData villageData;
        private UIVillageOptionsWindow uiVillageOptionsWindow;

        public void Init(SaveDataObject saveData, UIVillageOptionsWindow uiVillageOptionsWindow)
        {
            this.uiVillageOptionsWindow = uiVillageOptionsWindow;
            this.saveData = saveData;
        }


        public void OpenTransfer(SlaveDataCalculator slaveDataCalculator, VillageData villageData)
        {
            this.villageData = villageData;
            this.slaveDataCalculator = slaveDataCalculator;

            Redraw();

            ShowWindow();
        }

        private void Redraw()
        {
            foreach (var it in items)
            {
                Destroy(it.gameObject);
            }

            items.Clear();

            item.gameObject.SetActive(true);
            foreach (var islandData in saveData.Map.Islands)
            {
                if (islandData.IslandSeed != saveData.GetTargetIsland().IslandSeed)
                {
                    if (islandData.VillagesData.Count != 0)
                    {
                        Instantiate(item, item.transform.parent)
                            .With(x => items.Add(x))
                            .With(x => x.Init(islandData))
                            .With(x => x.Button.onClick.RemoveAllListeners())
                            .With(x => x.Button.onClick.AddListener(delegate { TransferToIsland(islandData); }));
                    }
                }
            }

            item.gameObject.SetActive(false);
        }

        private void TransferToIsland(SaveDataObject.MapData.IslandData islandData)
        {
            switch (slaveDataCalculator.CharacterData.SlaveData.TransferInfo.TransferState)
            {
                case ETransferState.NewOnIsland:
                    villageData.RemoveSlave(slaveDataCalculator.CharacterData.SlaveData.Uid);
                    break;
                case ETransferState.None:
                    slaveDataCalculator.CharacterData.SlaveData.TransferInfo.SetTransferState(ETransferState.SendFromIsland);
                    break;
            }

            foreach (var village in islandData.VillagesData)
            {
                var slave = village.GetSlave(slaveDataCalculator.CharacterData.SlaveData.Uid);

                if (slave != null)
                {
                    if (slave.TransferInfo.TransferState == ETransferState.SendFromIsland)
                    {
                        slave.TransferInfo.SetTransferState(ETransferState.None);
                        slave.SetActivityData(slaveDataCalculator.CharacterData.SlaveData.Activities);
                    }

                    SaveAndClose();
                    return;
                }
            }

            var newVillage = islandData.VillagesData.GetRandomItem();

            if (newVillage.GetSlave(slaveDataCalculator.CharacterData.Character.Uid) == null)
            {
                var transferedSlave = islandData.VillagesData.GetRandomItem().AddSlave(slaveDataCalculator.CharacterData.Character, new VillageData.SlaveData.TransferData(slaveDataCalculator.CharacterData.SlaveData.TransferInfo.Seed, slaveDataCalculator.CharacterData.SlaveData.TransferInfo.IslandLevel));
                transferedSlave.TransferInfo.SetTransferState(ETransferState.NewOnIsland);
                transferedSlave.SetActivityData(slaveDataCalculator.CharacterData.SlaveData.Activities);
                
                SaveAndClose();
            }
        }

        private void SaveAndClose()
        {
            uiVillageOptionsWindow.Redraw();
            saveData.SaveFile();
            CloseWindow();
        }
    }
}
