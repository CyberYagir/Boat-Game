using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class UIVillageInfoSubWindow: MonoBehaviour
    {
        [System.Serializable]
        public class WorkToggleButton
        {
            [SerializeField] private TMP_Text text;
            [SerializeField] private GameObject sleepIcon;
            [SerializeField] private GameObject workIcon;

            public void SetState(bool isWorking)
            {
                if (!isWorking)
                {
                    text.text = "Start Work";
                }
                else
                {
                    text.text = "Stop Work";
                }
                workIcon.gameObject.SetActive(isWorking);
                sleepIcon.gameObject.SetActive(!isWorking);
            }
        }

        [SerializeField] private UIBar staminaBar;
        [SerializeField] private WorkToggleButton workToggleButton;
        [SerializeField] private TMP_Dropdown storageTypeDropdown;
        [SerializeField] private TMP_Text openStorageText;

        [SerializeField] private UIVillageActionItem actionItem;
        [SerializeField] private List<UIVillageActionItem> actionItems;
        [SerializeField] private Button feedSlaveButton;
        [SerializeField] private Button killSlaveButton;
        [SerializeField] private Button makeHumanSlaveButton;
        [SerializeField] private Button openStorageSlaveButton;
        
        [SerializeField] private SlaveDataCalculator slaveDataCalculator = new SlaveDataCalculator();
        
        private SlaveData slaveData;
        private TickService tickService;
        private GameDataObject gameDataObject;
        private ResourcesService resourcesService;
        private SaveDataObject saveDataObject;
        private UIVillageManageSubWindow window;
        private UIMessageBoxManager messageBoxManager;

        public void Init(
            GameDataObject gameDataObject,
            SlaveData slaveData,
            DisplayCharacter slaveInfo,
            TickService tickService,
            ResourcesService resourcesService,
            SaveDataObject saveDataObject,
            UIVillageManageSubWindow window,
            UIMessageBoxManager messageBoxManager
        )
        {
            this.messageBoxManager = messageBoxManager;
            this.window = window;
            this.saveDataObject = saveDataObject;
            this.resourcesService = resourcesService;
            this.gameDataObject = gameDataObject;
            this.tickService = tickService;
            this.slaveData = slaveData;
            SpawnActivityItems(gameDataObject);
            staminaBar.Init("Energy", slaveData.TargetStamina, 100f);
            slaveDataCalculator.Init(slaveData, gameDataObject, slaveInfo);

            Redraw();


            tickService.OnTick += Tick;
        }

        private void Tick(float obj)
        {
            Redraw();
        }

        private void OnDisable()
        {
            tickService.OnTick -= Tick;
        }

        private void SpawnActivityItems(GameDataObject gameDataObject)
        {
            if (actionItems.Count == 0)
            {
                actionItem.gameObject.SetActive(true);
                foreach (var slaveActivitiesObject in gameDataObject.NativesListData.SlavesActivities)
                {
                    Instantiate(actionItem, actionItem.transform.parent)
                        .With(x => x.Init(slaveActivitiesObject, this))
                        .With(x => actionItems.Add(x));
                }
                actionItem.gameObject.SetActive(false);
            }
        }

        public void Redraw()
        {

            slaveDataCalculator.Recalculate();

            staminaBar.UpdateBar(slaveDataCalculator.ActualStamina);
            
            storageTypeDropdown.value = slaveData.IsStorage ? 1 : 0;
            workToggleButton.SetState(slaveData.IsWorking);


            killSlaveButton.interactable = makeHumanSlaveButton.interactable = !slaveData.IsWorking;
            feedSlaveButton.interactable = !slaveData.IsWorking && slaveData.TargetStamina < 100;
            openStorageSlaveButton.interactable = !slaveData.IsWorking && slaveDataCalculator.GetItemsCount() != 0;
            
            
            openStorageText.text = $"Open Storage ({slaveDataCalculator.GetItemsCount()})";
            
            for (int i = 0; i < actionItems.Count; i++)
            {
                actionItems[i].SetActive(slaveData.HasActivity(actionItems[i].Activity.Uid));
                actionItems[i].Enabled(!slaveData.IsWorking);
            }
        }


        public void WorkToggle()
        {
            slaveData.WorkToggle();

            if (!slaveData.IsWorking)
            {
                slaveDataCalculator.StopWorkAndSaveData();
                saveDataObject.SaveFile();

                if (slaveData.Activities.Count == 0)
                {
                    messageBoxManager.ShowMessageBox("Select the type of activity that the slave will engage in.", null, "Ok", "_disabled");
                }else if (slaveDataCalculator.ActualStamina <= 0)
                {
                    messageBoxManager.ShowMessageBox("Refill your slave's stamina by feeding him.", null, "Ok", "_disabled");
                }
            }
            
            Redraw();
        }

        public void DropDownChange(int value)
        {
            slaveData.SetStorage(value == 1);
        }

        public void FeedSlaveButton()
        {
            if (slaveData.TargetStamina < 100f)
            {
                var eat = resourcesService.AllItemsList.Find(x => x.Item.Type == EResourceTypes.Eat);
                if (eat != null)
                {
                    resourcesService.RemoveItemFromAnyRaft(eat.Item);
                    slaveDataCalculator.AddEat(eat.Item.ParametersData.Hunger * gameDataObject.ConfigData.SlaveEatEfficiencyMultiplier);
                    Redraw();
                }
            }
        }

        public void ToggleActivity(SlaveActivitiesObject activity)
        {
            slaveData.ToggleActivity(activity);
            Redraw();
        }

        public void KillSlaveButton()
        {
            messageBoxManager.ShowMessageBox("Are you sure you want to kill the slave? Perhaps he has children and a family...", delegate
            {
                window.KillSlave(slaveData.Uid);
            }, "Execute");
        }

        public void OpenStorageButton()
        {
            if (slaveData.IsWorking)
            {
                WorkToggle();
            }
            window.OpenSlaveStorage(slaveDataCalculator);
        }
    }
}