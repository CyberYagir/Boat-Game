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
        public float ProgressPercent => smeltedItem.Item != null && smeltedItem.Count != 0 ? currentProgressTicks / (float)(SmeltedItem.Item.FurnaceData.SmeltSeconds * TimeService.Ticks) : 0;

        [Inject]
        private void Construct(TickService tickService)
        {
            tickService.OnTick += OnTick;
        }

        private void OnTick(float obj)
        {
            ProgressUpdate();
            FuelUpdate();
        }

        private void ProgressUpdate()
        {
            if (smeltedItem.Item == null || smeltedItem.Count == 0)
            {
                currentProgressTicks = 0;
                return;
            }

            if (resultItem.Item != null && resultItem.Item != smeltedItem.Item.FurnaceData.AfterSmeltItem)
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
                    }
                }
            }

        }

        public void LoadStorage()
        {
            
        }

        public void SetFuel(RaftStorage.StorageItem storageItem)
        {
            fuelItem = storageItem;
        }

        public void SetSmelt(RaftStorage.StorageItem storageItem)
        {
            smeltedItem = storageItem;
        }
    }
}
