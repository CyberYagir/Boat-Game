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
        
        [System.Serializable]
        public class TooltipInit
        {
            [SerializeField] private UITooltip tooltip;
            [SerializeField] private TooltipDataObject tooltipDataObject;

            public void Init()
            {
                tooltip.Init(tooltipDataObject);
            }
        }
        
        [SerializeField] private WorkToggleButton workToggleButton;
        [SerializeField] private SlaveDataCalculator slaveDataCalculator = new SlaveDataCalculator();
        
        [SerializeField] private UIBar staminaBar;
        [SerializeField] private TMP_Dropdown storageTypeDropdown;
        [SerializeField] private TMP_Text openStorageText;

        [SerializeField] private UIVillageActionItem actionItem;
        [SerializeField] private Button feedSlaveButton;
        [SerializeField] private Button killSlaveButton;
        [SerializeField] private Button makeHumanSlaveButton;
        [SerializeField] private Button openStorageSlaveButton;
        
        [SerializeField] private List<TooltipInit> tooltips;
        [SerializeField] private List<UIVillageActionItem> actionItems;
        
        private TickService tickService;
        private GameDataObject gameDataObject;
        private ResourcesService resourcesService;
        private SaveDataObject saveDataObject;
        private UIVillageManageSubWindow window;
        private UIMessageBoxManager messageBoxManager;
        private SlaveCreatedCharacterInfo slaveInfo;

        private SlaveData SlaveData => slaveInfo.SlaveData;
        

        public void Init(
            GameDataObject gameDataObject,
            SlaveCreatedCharacterInfo slaveInfo,
            TickService tickService,
            ResourcesService resourcesService,
            SaveDataObject saveDataObject,
            UIVillageManageSubWindow window,
            UIMessageBoxManager messageBoxManager
        )
        {
            this.slaveInfo = slaveInfo;
            this.messageBoxManager = messageBoxManager;
            this.window = window;
            this.saveDataObject = saveDataObject;
            this.resourcesService = resourcesService;
            this.gameDataObject = gameDataObject;
            this.tickService = tickService;
            
            SpawnActivityItems(gameDataObject);
            staminaBar.Init("Energy", SlaveData.TargetStamina, 100f);
            slaveDataCalculator.Init(SlaveData, gameDataObject, slaveInfo);

            for (int i = 0; i < tooltips.Count; i++)
            {
                tooltips[i].Init();
            }
            
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
            
            storageTypeDropdown.value = SlaveData.IsStorage ? 1 : 0;
            workToggleButton.SetState(SlaveData.IsWorking);


            killSlaveButton.interactable = makeHumanSlaveButton.interactable = !SlaveData.IsWorking;
            feedSlaveButton.interactable = !SlaveData.IsWorking && SlaveData.TargetStamina < 100;
            openStorageSlaveButton.interactable = !SlaveData.IsWorking && slaveDataCalculator.GetItemsCount() != 0;
            
            
            openStorageText.text = $"Open Storage ({slaveDataCalculator.GetItemsCount()})";
            
            for (int i = 0; i < actionItems.Count; i++)
            {
                actionItems[i].SetActive(SlaveData.HasActivity(actionItems[i].Activity.Uid));
                actionItems[i].Enabled(!SlaveData.IsWorking);
            }
        }


        public void WorkToggle()
        {
            var isWorking = SlaveData.IsWorking;
            SlaveData.WorkToggle();

            if (!SlaveData.IsWorking)
            {
                slaveDataCalculator.StopWorkAndSaveData();
                saveDataObject.SaveFile();

                if (isWorking == false)
                {
                    if (SlaveData.Activities.Count == 0)
                    {
                        messageBoxManager.ShowMessageBox("Select the type of activity that the slave will engage in.", null, "Ok", "_disabled");
                    }
                    else if (slaveDataCalculator.ActualStamina <= 0)
                    {
                        messageBoxManager.ShowMessageBox("Refill your slave's stamina by feeding him.", null, "Ok", "_disabled");
                    }
                }
            }
            
            Redraw();
        }

        public void DropDownChange(int value)
        {
            SlaveData.SetStorage(value == 1);
        }

        public void FeedSlaveButton()
        {
            if (SlaveData.TargetStamina < 100f)
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
            SlaveData.ToggleActivity(activity);
            Redraw();
        }

        public void KillSlaveButton()
        {
            messageBoxManager.ShowMessageBox("Are you sure you want to kill the slave? Perhaps he has children and a family...", delegate
            {
                window.KillSlave(slaveInfo);
            }, "Execute");
        }

        public void OpenStorageButton()
        {
            if (SlaveData.IsWorking)
            {
                WorkToggle();
            }
            window.OpenSlaveStorage(slaveDataCalculator);
        }

        public void TransferButton()
        {
          
            if (slaveDataCalculator.GetItemsCount() != 0)
            {
                messageBoxManager.ShowMessageBox("Free the slave from things, and then it can be moved.", null, "Ok", "_disabled");
                return;
            }
            
            if (SlaveData.IsWorking)
            {
                WorkToggle();
            }

            window.TransferSlave(slaveDataCalculator);

        }
    }
}