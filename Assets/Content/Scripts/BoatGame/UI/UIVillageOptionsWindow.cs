using System;
using System.Numerics;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.Scriptable;
using Content.Scripts.ManCreator;
using UnityEngine;
using static Content.Scripts.Global.SaveDataObject.MapData.IslandData.VillageData.SlaveData;
using Random = System.Random;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageOptionsWindow : AnimatedWindow
    {
        [SerializeField] private UIVillageSocialRatingCounter villageSocialRating;
        [SerializeField] private UIVillageTradeSubWindow tradeSubWindow;
        [SerializeField] private UIVillageSlavesGenerator slavesGenerator;
        [SerializeField] private UIVillageSlavesSubWindow slavesSubWindow;
        [SerializeField] private UIVillageManageSubWindow manageSubWindow;
        [SerializeField] private UIVillageStorageSubWindow storageSubWindow;
        [SerializeField] private UIVillageTransferSubWindow transferSubWindow;
        [SerializeField] private UIVillageFightsSubWindow fightsSubWindow;


        private RaftBuildService raftBuildService;
        private SaveDataObject saveDataObject;
        private SaveDataObject.MapData.IslandData.VillageData villageData;
        private GameDataObject gameDataObject;
        private ResourcesService resourcesService;
        private PlayerCharacter targetPlayer;
        private TickService tickService;
        private UIService uiService;
        private UIMessageBoxManager messageBoxManager;
        private ScenesService scenesService;
        private SaveService saveService;

        public void Init(
            SelectionService selectionService,
            RaftBuildService raftBuildService,
            SaveDataObject saveDataObject,
            GameDataObject gameDataObject,
            ResourcesService resourcesService,
            TickService tickService,
            UIService uiService,
            UIMessageBoxManager messageBoxManager,
            ScenesService scenesService,
            SaveService saveService
        )
        {
            this.saveService = saveService;
            this.scenesService = scenesService;
            this.messageBoxManager = messageBoxManager;
            this.uiService = uiService;
            this.tickService = tickService;
            this.resourcesService = resourcesService;
            this.gameDataObject = gameDataObject;
            this.saveDataObject = saveDataObject;
            this.raftBuildService = raftBuildService;

            villageSocialRating.Init(gameDataObject.TradesData);
            storageSubWindow.Init(resourcesService);
            transferSubWindow.Init(saveDataObject, this);
            
            raftBuildService.OnChangeRaft += ChangeRaftsEvents;
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
        }



        private void ChangeRaftsEvents()
        {
            foreach (var storage in raftBuildService.Storages)
            {
                storage.OnStorageChange -= StorageOnOnStorageChange;
                storage.OnStorageChange += StorageOnOnStorageChange;
            }
        }

        private void StorageOnOnStorageChange()
        {
            Redraw();
        }

        public void Redraw()
        {
            if (villageData == null) return;

            villageSocialRating.Redraw(villageData.SocialRating);
            tradeSubWindow.Redraw();
            manageSubWindow.Redraw();
        }

        public override void ShowWindow()
        {
            base.ShowWindow();

            Redraw();
            if (targetPlayer)
            {
                targetPlayer.NeedManager.SetGodMode();
            }

            uiService.SetResourcesCounterSorting(1);

        }

        public void ShowAndSetVillage(string villageID, int level)
        {
            villageData = saveDataObject.GetTargetIsland().GetVillage(villageID);
            villageData.OnChangeSocialRaiting += UpdateRatingCounter;
            villageSocialRating.Redraw(villageData.SocialRating);
            
            
            var rnd = villageData.GetRandom();
            tradeSubWindow.Init(
                gameDataObject.TradesData.GetRandomSellsByLevel(level, rnd),
                gameDataObject.TradesData.GetRandomBuysByLevel(level, rnd),
                this,
                resourcesService
            );

            rnd = villageData.GetRandom();
            slavesGenerator.Init(villageData, gameDataObject, rnd, level);
            slavesGenerator.Show();
            slavesSubWindow.Init(gameDataObject, resourcesService, slavesGenerator, this);
            manageSubWindow.Init(slavesGenerator, villageData, gameDataObject, tickService, resourcesService, saveDataObject, this, messageBoxManager);
            fightsSubWindow.Init(villageData, gameDataObject, level);
            
            
            ShowWindow();
        }

        private void UpdateRatingCounter(int value)
        {
            villageSocialRating.Redraw(value);
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            targetPlayer = obj;
            if (obj == null) return;
            var furnaceAction = obj.GetCharacterAction<CharActionViewVillage>();
            furnaceAction.OnOpenWindow -= ShowAndSetVillage;
            furnaceAction.OnOpenWindow += ShowAndSetVillage;

            obj.NeedManager.OnDeath -= OnDeath;
            obj.NeedManager.OnDeath += OnDeath;

        }

        private void OnDeath(Character obj)
        {
            CloseWindow();
        }

        public override void CloseWindow()
        {
            base.CloseWindow();
            if (villageData != null)
            {
                villageData.OnChangeSocialRaiting -= UpdateRatingCounter;
                slavesGenerator.Hide();
            }

            if (targetPlayer != null)
            {
                targetPlayer.NeedManager.SetGodMode(false);
            }
            uiService.SetResourcesCounterSorting(0);
        }

        public void SellItem(RaftStorage.StorageItem sellItem, RaftStorage.StorageItem resultItem, TradeOfferObject tradeOfferObject)
        {
            if (resourcesService.IsCanTradeItem(sellItem, resultItem))
            {
                if (resourcesService.IsHaveItem(sellItem))
                {
                    resourcesService.RemoveItemsFromAnyRaft(sellItem);
                    resourcesService.AddItemsToAnyRafts(resultItem);
                    villageData.AddSocialRating(tradeOfferObject.SocialRatingPoints + UnityEngine.Random.Range(0, 15));

                    Redraw();
                }
            }
        }

        public float GetActualModify()
        {
            return gameDataObject.TradesData.GetEmotionalData(villageData.SocialRating).TradeMultiply;
        }

        public bool BuySlave(SlaveCreatedCharacterInfo info)
        {
            var item = new RaftStorage.StorageItem(gameDataObject.ConfigData.MoneyItem, info.Cost);
            if (resourcesService.IsHaveItem(item))
            {
                var createdSlave = villageData.AddSlave(info.Character, new TransferData(info.Seed, info.IslandLevel));
                info.SetSlaveData(createdSlave);
                villageData.AddSocialRating(info.Cost);
                resourcesService.RemoveItemsFromAnyRaft(item);
                saveDataObject.SaveFile();
                Redraw();
                return true;
            }

            return false;
        }

        public SaveDataObject.MapData.IslandData.VillageData GetVillage()
        {
            return villageData;
        }

        public void OpenSlaveStorage(SlaveDataCalculator slaveDataCalculator)
        {
            storageSubWindow.OpenSlaveStorage(slaveDataCalculator);
        }

        public void TransferSlave(SlaveDataCalculator slaveDataCalculator)
        {
            transferSubWindow.OpenTransfer(slaveDataCalculator, villageData);
        }

        public void HealSlave(SaveDataObject.MapData.IslandData.VillageData.SlaveData slaveData)
        {
            CharacterCustomizationService.SetTargetSlave(slaveData.Uid);
            resourcesService.RemoveItemFromAnyRaft(gameDataObject.ConfigData.HealSlaveItem);
            saveService.SaveWorld();
            scenesService.FadeScene(ESceneName.ManCreator);
        }
    }
}
