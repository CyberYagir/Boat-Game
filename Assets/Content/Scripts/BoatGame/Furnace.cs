using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class Furnace : MonoBehaviour
    {
        [SerializeField] private RaftStorage.StorageItem smeltedItem;
        [SerializeField] private RaftStorage.StorageItem fuelItem;
        [SerializeField] private RaftStorage.StorageItem resultItem;

        [SerializeField, ReadOnly] private int currentMaxFuelTicks;
        [SerializeField, ReadOnly] private int currentFuelTicks = 0;
        [SerializeField, ReadOnly] private int currentProgressTicks = 0;

        public RaftStorage.StorageItem ResultItem => resultItem;

        public RaftStorage.StorageItem FuelItem => fuelItem;

        public RaftStorage.StorageItem SmeltedItem => smeltedItem;

        public float FuelPercent => currentFuelTicks / (float) currentMaxFuelTicks;
        public float ProgressPercent => smeltedItem.Item != null && smeltedItem.Count != 0 ? currentProgressTicks / (float) (SmeltedItem.Item.FurnaceData.SmeltSeconds * TimeService.Ticks) : 0;
        public int ProgressionTicks => currentProgressTicks;
        public int FuelTicks => currentFuelTicks;
        public int MaxFuelTicks => currentMaxFuelTicks;

        public event Action<bool> OnFurnaceStateChange;


        [Inject]
        private void Construct(TickService tickService)
        {
            tickService.OnTick += OnTick;
        }

        private void OnTick(float obj)
        {
            ReleaseSlots();
            ProgressUpdate();
            FuelUpdate();
        }

        private void ReleaseSlots()
        {
            if (fuelItem.Item != null && fuelItem.Count == 0)
            {
                fuelItem = new RaftStorage.StorageItem(null, 0);
            }

            if (resultItem.Item != null && resultItem.Count == 0)
            {
                resultItem = new RaftStorage.StorageItem(null, 0);
            }

            if (smeltedItem.Item != null && smeltedItem.Count == 0)
            {
                smeltedItem = new RaftStorage.StorageItem(null, 0);
            }
        }

        private void ProgressUpdate()
        {


            if (smeltedItem.Item == null || smeltedItem.Count == 0)
            {
                currentProgressTicks = 0;
                return;
            }

            if ((resultItem.Item != null && resultItem.Count != 0) && resultItem.Item != smeltedItem.Item.FurnaceData.AfterSmeltItem)
            {
                currentProgressTicks = 0;
                return;
            }

            if (currentFuelTicks > 0)
            {
                currentProgressTicks++;
            }
            else
            {
                currentProgressTicks--;
                if (currentProgressTicks < 0)
                {
                    currentProgressTicks = 0;
                }
            }

            if (currentProgressTicks > smeltedItem.Item.FurnaceData.SmeltSeconds * TimeService.Ticks)
            {
                smeltedItem.Add(-1);
                if (smeltedItem.Item.FurnaceData.AfterSmeltItem != resultItem.Item)
                {
                    resultItem = new RaftStorage.StorageItem(null, 0);
                }

                if (resultItem.Item == null)
                {
                    resultItem = new RaftStorage.StorageItem(smeltedItem.Item.FurnaceData.AfterSmeltItem, 0);
                }

                resultItem.Add(1);

                currentProgressTicks = 0;
            }
        }

        private void FuelUpdate()
        {
            if (currentFuelTicks == 1)
            {
                OnFurnaceStateChange?.Invoke(false);
            }

            currentFuelTicks--;
            if (currentFuelTicks <= 0)
            {
                currentFuelTicks = 0;
                if (fuelItem.Item != null)
                {
                    if (fuelItem.Count > 0)
                    {
                        fuelItem.Add(-1);
                        currentFuelTicks = (int) (fuelItem.Item.FurnaceData.FuelSeconds * TimeService.Ticks);
                        currentMaxFuelTicks = currentFuelTicks;

                        OnFurnaceStateChange?.Invoke(true);
                    }
                }
            }

        }
        public void SetFuel(RaftStorage.StorageItem storageItem)
        {
            fuelItem = storageItem;
        }

        public void SetSmelt(RaftStorage.StorageItem storageItem)
        {
            smeltedItem = storageItem;
        }
        
        public void SetResult(RaftStorage.StorageItem storageItem)
        {
            resultItem = storageItem;
        }

        public void LoadStorage(SaveDataObject.RaftsData.RaftFurnace item, GameDataObject gameData)
        {
            if (item == null) return;
            
            currentFuelTicks = item.FuelTicks;
            currentMaxFuelTicks = item.MaxFuelTicks;
            currentProgressTicks = item.ProgressTicks;

            smeltedItem = new RaftStorage.StorageItem(gameData.GetItem(item.SmeltItem.ItemID), item.SmeltItem.Count);
            fuelItem = new RaftStorage.StorageItem(gameData.GetItem(item.FuelItem.ItemID), item.FuelItem.Count);
            resultItem = new RaftStorage.StorageItem(gameData.GetItem(item.ResultItem.ItemID), item.ResultItem.Count);

            if (currentFuelTicks > 0)
            {
                OnFurnaceStateChange?.Invoke(true);
            }
        }

     
    }
}
