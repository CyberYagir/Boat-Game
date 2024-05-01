using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionPickup : CharActionMoveTo
    {
        
        private float pickUpTime;
        private bool isMoving;
        protected Vector3 currentTarget;

        private DroppedItem droppedItem;

        protected bool dontStartIfDroppedItemNull = true;
        
        
        public override void ResetState()
        {
            base.ResetState();
            
            pickUpTime = 0;
            isMoving = true;
        }


        public override void StartState()
        {
            base.StartState();

            currentTarget = SelectionService.LastWorldClick;

            droppedItem = SelectionService.SelectedObject.Transform.GetComponent<DroppedItem>();

            if (droppedItem == null && dontStartIfDroppedItemNull)
            {
                EndState();
            }
        }

        public override void ProcessState()
        {
            base.ProcessState();

            if (!isMoving)
            {
                pickUpTime += TimeService.DeltaTime;

                if (droppedItem == null && dontStartIfDroppedItemNull)
                {
                    EndState();
                }
                
                if (pickUpTime >= 1f)
                {
                    AddToInventory();

                    EndState();
                }
            }
        }

        public virtual void AddToInventory()
        {
            if (droppedItem)
            {
                var storage = Machine.AIMoveManager.GoToEmptyStorage(1);
                
                if (storage != null)
                {
                    storage.AddToStorage(droppedItem.Item, 1, false);
                    Machine.AddExp(1);
                    WorldPopupService.StaticSpawnPopup(droppedItem.transform.position, droppedItem.Item, 1);
                    droppedItem.DeleteItem();
                }
                else
                {
                    
                    WorldPopupService.StaticSpawnCantPopup(droppedItem.transform.position);
                }
            }
        }


        protected override void OnMoveEnded()
        {
            if (isMoving)
            {
                Machine.AnimationManager.TriggerPickUpAnim();
                isMoving = false;
            }
        }

        public override void EndState()
        {
            base.EndState();
            ToIdleAnimation();
        }
    }
}
