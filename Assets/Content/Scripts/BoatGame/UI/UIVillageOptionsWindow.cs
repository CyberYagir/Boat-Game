using System;
using System.Numerics;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.Scriptable;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageOptionsWindow : AnimatedWindow
    {
        [SerializeField] private UIVillageSocialRatingCounter villageSocialRating;
        [SerializeField] private UIVillageTradeSubWindow tradeSubWindow;
        [SerializeField] private UIVillageSlavesSubWindow slavesSubWindow;
        private RaftBuildService raftBuildService;
        private SaveDataObject saveDataObject;
        private SaveDataObject.MapData.IslandData.VillageData villageData;
        private GameDataObject gameDataObject;
        private ResourcesService resourcesService;
        private PlayerCharacter targetPlayer;

        public void Init(
            SelectionService selectionService,
            RaftBuildService raftBuildService,
            SaveDataObject saveDataObject,
            GameDataObject gameDataObject,
            ResourcesService resourcesService
        )
        {
            this.resourcesService = resourcesService;
            this.gameDataObject = gameDataObject;
            this.saveDataObject = saveDataObject;
            this.raftBuildService = raftBuildService;

            villageSocialRating.Init(gameDataObject.TradesData);


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

        private void Redraw()
        {
            if (villageData == null) return;

            villageSocialRating.Redraw(villageData.SocialRating);
            tradeSubWindow.Redraw();
            
        }

        public override void ShowWindow()
        {
            base.ShowWindow();

            Redraw();
            if (targetPlayer)
            {
                targetPlayer.NeedManager.SetGodMode();
            }
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
            slavesSubWindow.Init(gameDataObject, saveDataObject, rnd, level, resourcesService, this);

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
            }

            if (targetPlayer != null)
            {
                targetPlayer.NeedManager.SetGodMode(false);
            }
        }

        public void SellItem(RaftStorage.StorageItem sellItem, RaftStorage.StorageItem resultItem, TradeOfferObject tradeOfferObject)
        {
            if (resourcesService.IsCanTradeItem(sellItem, resultItem))
            {
                if (resourcesService.IsHaveItem(sellItem))
                {
                    resourcesService.RemoveItemsFromAnyRaft(sellItem);
                    resourcesService.AddItemsToAnyRafts(resultItem);
                    villageData.AddSocialRating(tradeOfferObject.SocialRatingPoints);

                    Redraw();
                }
            }
        }

        public float GetActualModify()
        {
            return gameDataObject.TradesData.GetEmotionalData(villageData.SocialRating).TradeMultiply;
        }

        public bool BuySlave(Character character, int cost)
        {
            var item = new RaftStorage.StorageItem(gameDataObject.ConfigData.MoneyItem, cost);
            if (resourcesService.IsHaveItem(item))
            {
                villageData.AddSlave(character);
                resourcesService.RemoveItemsFromAnyRaft(item);
                saveDataObject.SaveFile();

                return true;
            }

            return false;
        }

        public SaveDataObject.MapData.IslandData.VillageData GetVillage()
        {
            return villageData;
        }
    }
}
