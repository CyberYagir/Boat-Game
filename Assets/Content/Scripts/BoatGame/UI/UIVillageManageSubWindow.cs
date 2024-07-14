using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageManageSubWindow : MonoBehaviour
    {
        [SerializeField] private UIVillageManageCharItem charactersListItem;
        [SerializeField] private GameObject infoWindow;
        [SerializeField] private UIVillageInfoSubWindow uiVillageInfoWindowData;
        [SerializeField] private TMP_Text emptyText;
        
        private List<UIVillageManageCharItem> charactersListItems = new List<UIVillageManageCharItem>();
        private UIVillageSlavesVisualsGenerator generator;
        private SaveDataObject.MapData.IslandData.VillageData villageData;
        private DisplayCharacter selectedCharacter;
        private GameDataObject gameDataObject;
        private TickService tickService;
        private ResourcesService resourcesService;
        private SaveDataObject saveData;
        private UIVillageOptionsWindow window;
        private UIMessageBoxManager messageBoxManager;


        public void Init(
            UIVillageSlavesVisualsGenerator generator,
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

        public void SelectCharacter(DisplayCharacter character)
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
                var targetSlave = villageData.GetSlave(selectedCharacter.Character.Uid);

                if (targetSlave != null && !targetSlave.IsDead)
                {
                    uiVillageInfoWindowData.Init(gameDataObject, targetSlave, selectedCharacter, tickService, resourcesService, saveData, this, messageBoxManager);
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
                charactersListItems[i].gameObject.SetActive(i < villageData.SlavesCount());
                if (i < villageData.SlavesCount())
                {
                    var slave = villageData.GetSlaveByID(i);
                    if (slave != null)
                    {
                        if (!slave.IsDead)
                        {
                            var character = generator.Characters.Find(x => x.Character.Uid == slave.Uid);
                            if (character != null)
                            {
                                charactersListItems[i].Init(character, this, selectedCharacter == character);
                                counter++;
                            }
                        }
                        else
                        {
                            charactersListItems[i].gameObject.SetActive(false);
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

        public void KillSlave(string slaveDataUid)
        {
            if (selectedCharacter.Character.Uid == slaveDataUid)
            {
                selectedCharacter = null;
            }

            villageData.KillSlave(slaveDataUid);
            
            Redraw();
        }
    }
}
