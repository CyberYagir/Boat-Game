using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Sources;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionGetFromSource : CharActionPickup
    {
        private Vector3 lastPoint;
        private ISourceObject targetSource;
        
        public override void StartState()
        {
            Agent.SetStopped(false);
            
            dontStartIfDroppedItemNull = false;
            lastPoint = SelectionService.LastWorldClick;
            targetSource = SelectionService.SelectedObject.Transform.GetComponent<ISourceObject>();

            if (targetSource == null)
            {
                EndState();
                return;
            }
            
            if (!MoveToPoint(SelectionService.SelectedObject.Transform.position))
            {
                EndState();
            }
        }

        public override void AddToInventory()
        {
            ItemObject item = targetSource.GetFromItem();
            if (item != null)
            {
                var storage = Machine.AIMoveManager.GoToEmptyStorage(1);
                if (storage != null)
                {
                    storage.AddToStorage(item, 1, false);
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
