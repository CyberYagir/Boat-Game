using System;
using System.Collections.Generic;
using System.Globalization;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class SlaveDataCalculator
    {
        private DateTime endOfWorkingTime;
        private DateTime targetSlaveTimeProgress;
        
        [SerializeField, ReadOnly] private List<RaftStorage.StorageItem> items = new List<RaftStorage.StorageItem>(20);
        private List<RaftStorage.StorageItem> storage = new List<RaftStorage.StorageItem>(20);
        private SaveDataObject.MapData.IslandData.VillageData.SlaveData slaveData;
        private ConfigDataObject config;
        private GameDataObject gameDataObject;
        private DisplayCharacter characterData;


        public float ActualStamina => slaveData.IsWorking ? CalculateStamina() : slaveData.TargetStamina;

        public DisplayCharacter CharacterData => characterData;

        public void Init(SaveDataObject.MapData.IslandData.VillageData.SlaveData slaveData, GameDataObject gameDataObject, DisplayCharacter characterData)
        {
            this.characterData = characterData;
            this.gameDataObject = gameDataObject;
            this.config = gameDataObject.ConfigData;
            this.slaveData = slaveData;
            RecalculateStorageItems();
        }

        private void RecalculateStorageItems()
        {
            storage.Clear();
            foreach (var slaveDataStorageItem in this.slaveData.StorageItems)
            {
                storage.Add(new RaftStorage.StorageItem(gameDataObject.GetItem(slaveDataStorageItem.ItemID), slaveDataStorageItem.Count));
            }
        }

        public void Recalculate()
        {
            var rndSeed = new Random(slaveData.LastTimeStampString.ToString(CultureInfo.InvariantCulture).GetHashCode());
            
            TimeCalculation();

            StorageCalculations(gameDataObject.NativesListData.SlavesActivities, rndSeed);
        }

        private void StorageCalculations(List<SlaveActivitiesObject> slaveActivitiesObjects, Random rndSeed)
        {
            items.Clear();

            if (!slaveData.IsWorking) return;
            
            
            for (int i = 0; i < slaveData.Activities.Count; i++)
            {
                var activity = slaveActivitiesObjects.Find(x => x.Uid == slaveData.Activities[i].ActivityID);
                if (activity != null)
                {
                    var itemsCount = (targetSlaveTimeProgress - slaveData.LastTimeStamp).TotalSeconds / (activity.ItemsPerTime * slaveData.Activities.Count);
                    if (itemsCount >= 1)
                    {
                        for (int j = 0; j < itemsCount; j++)
                        {
                            var item = activity.GetActivityResourcesByTime(rndSeed, slaveData.IsStorage);

                            if (item != null)
                            {
                                var storageItem = items.Find(x => x.Item == item.Item);
                                if (storageItem == null)
                                {
                                    items.Add(item);
                                }
                                else
                                {
                                    storageItem.Add(item.Count);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void TimeCalculation()
        {
            endOfWorkingTime = slaveData.LastTimeStamp.AddSeconds(config.SlaveStaminaWorkingPerSecond * slaveData.TargetStamina);

            if (DateService.ActualDate > endOfWorkingTime)
            {
                targetSlaveTimeProgress = endOfWorkingTime;
            }
            else
            {
                targetSlaveTimeProgress = DateService.ActualDate;
            }
        }

        public void StopWorkAndSaveData()
        {
            var newStamina = CalculateStamina();
            slaveData.SetStamina(newStamina);
            slaveData.AddItemsToStorage(items);
            RecalculateStorageItems();
            items.Clear();
        }

        private float CalculateStamina()
        {
            if ((targetSlaveTimeProgress - slaveData.LastTimeStamp).TotalSeconds == 0)
            {
                return 0;
            }
            
            var staminaPercent = ((targetSlaveTimeProgress - slaveData.LastTimeStamp).TotalSeconds) / (config.SlaveStaminaWorkingPerSecond * slaveData.TargetStamina);
            var newStamina = slaveData.TargetStamina * (1f - (float)staminaPercent);
            return newStamina;
        }

        public void AddEat(float eatPoints)
        {
            slaveData.AddStamina(eatPoints);
        }
        
        public List<RaftStorage.StorageItem> GetStorage()
        {
            return storage;
        }

        public bool RemoveItem(RaftStorage.StorageItem availableItem)
        {
            bool isRemoved = slaveData.RemoveItem(availableItem);
            RecalculateStorageItems();

            return isRemoved;
        }
    }
}