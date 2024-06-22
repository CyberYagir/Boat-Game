using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionGetGrass : CharActionPickup
    {
        private Vector3 lastPoint;
        [SerializeField] private ItemObject[] items;

        public override void StartState()
        {
            dontStartIfDroppedItemNull = false;
            base.StartState();
        }

        public override void AddToInventory()
        {
            if (lastPoint.ToDistance(currentTarget) > 2)
            {
                var item = items.GetRandomItem();
                var storage = Machine.AIMoveManager.GoToEmptyStorage(item, 1);
                if (storage != null)
                {
                    storage.AddToStorage(item, 1, false);
                    lastPoint = currentTarget;
                    WorldPopupService.StaticSpawnPopup(lastPoint, item, 1);
                    Machine.AddExp(1);
                    
                }
                else
                {
                    WorldPopupService.StaticSpawnCantPopup(currentTarget);
                }
            }
            else
            { 
                WorldPopupService.StaticSpawnCantPopup(currentTarget);
            }
        }
    }
}
