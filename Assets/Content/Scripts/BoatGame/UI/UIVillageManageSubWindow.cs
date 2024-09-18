using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData.SlaveData.TransferData;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageManageSubWindow : MonoBehaviour
    {
        [SerializeField] private UIVillageManageCharItem charactersListItem;
        [SerializeField] private GameObject infoWindow;
        [SerializeField] private UIVillageInfoSubWindow uiVillageInfoWindowData;
        [SerializeField] private TMP_Text emptyText;
        [SerializeField, ReadOnly] private List<UIVillageManageCharItem> charactersListItems = new List<UIVillageManageCharItem>();
        
        private UIVillageSlavesGenerator generator;
        private SaveDataObject.MapData.IslandData.VillageData villageData;
        private SlaveCreatedCharacterInfo selectedCharacter;
        private GameDataObject gameDataObject;
        private TickService tickService;
        private ResourcesService resourcesService;
        private SaveDataObject saveData;
        private UIVillageOptionsWindow window;
        private UIMessageBoxManager messageBoxManager;


        public void Init(
            UIVillageSlavesGenerator generator,
            SaveDataObject.MapData.IslandData.VillageData villageData,
            GameDataObject gameDataObject,
            TickService tickService,
            ResourcesService resourcesService,
            SaveDataObject saveData,
            UIVillageOptionsWindow window,
            UIMessageBoxManager messageBoxManager
        )
        {
            this.messageBoxManager = messageBoxManager;
            this.window = window;
            this.saveData = saveData;
            this.resourcesService = resourcesService;
            this.tickService = tickService;
            this.gameDataObject = gameDataObject;
            this.villageData = villageData;
            this.generator = generator;

            Redraw();
        }

        public void SelectCharacter(SlaveCreatedCharacterInfo character)
        {
            selectedCharacter = character;
            Redraw();
        }


        public void Redraw()
        {
            RedrawSlavesList();
            UpdateSlavesList();

            if (selectedCharacter != null)
            {
                var targetSlave = generator.SlavesInfos.Find(x => x.Character.Uid == selectedCharacter.Character.Uid);
                
                if (targetSlave != null && !targetSlave.IsDead)
                {
                    uiVillageInfoWindowData.Init(gameDataObject, targetSlave, tickService, resourcesService, saveData, this, messageBoxManager);
                }

                if (targetSlave != null && targetSlave.IsDead)
                {
                    infoWindow.SetActive(false);
                    return;
                }
            }


            infoWindow.SetActive(selectedCharacter != null);
        }

        private void RedrawSlavesList()
        {
            charactersListItem.gameObject.SetActive(true);
            for (int i = 0; i < villageData.SlavesCount() - charactersListItems.Count; i++)
            {
                Instantiate(charactersListItem, charactersListItem.transform.parent)
                    .With(x => charactersListItems.Add(x));
            }

            charactersListItem.gameObject.SetActive(false);
        }

        private void UpdateSlavesList()
        {
            int counter = 0;
            for (int i = 0; i < charactersListItems.Count; i++)
            {
                charactersListItems[i].gameObject.SetActive(false);
                if (i < villageData.SlavesCount())
                {
                    var slave = villageData.GetSlaveByID(i);
                    if (slave != null)
                    {
                        if (villageData.IsHaveSlave(slave.Uid) && !slave.IsDead)
                        {
                            if (slave.TransferInfo.TransferState is not ETransferState.SendFromIsland and not ETransferState.Hired)
                            {
                                var infoIndex = generator.SlavesInfos.FindIndex(x => x.Character.Uid == slave.Uid);
                                if (infoIndex != -1)
                                {
                                    var character = generator.SlavesVisuals[infoIndex];
                                    var info = generator.SlavesInfos[infoIndex];
                                    if (character != null)
                                    {
                                        charactersListItems[i].gameObject.SetActive(true);

                                        charactersListItems[i].Init(character, info, this, selectedCharacter == info);
                                        counter++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            emptyText.gameObject.SetActive(counter == 0);
        }

        public void OpenSlaveStorage(SlaveDataCalculator slaveDataCalculator)
        {
            window.OpenSlaveStorage(slaveDataCalculator);
        }

        public void KillSlave(SlaveCreatedCharacterInfo info)
        {
            if (selectedCharacter.Character.Uid == info.Character.Uid)
            {
                selectedCharacter = null;
            }
            
            saveData.CrossGame.AddSoul();
            villageData.KillSlave(info.Character.Uid);
            villageData.AddSocialRating(info.Cost / 2);
            

            
            Redraw();
        }

        public void TransferSlave(SlaveDataCalculator slaveDataCalculator)
        {
            window.TransferSlave(slaveDataCalculator);
            selectedCharacter = null;
        }

        public void HealSlave(SaveDataObject.MapData.IslandData.VillageData.SlaveData slaveData)
        {
            window.HealSlave(slaveData);
        }
    }
}
