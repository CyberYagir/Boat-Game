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

        private DroppedItemBase droppedItem;
        private DroppedItemBase cachedItem;

        protected bool dontStartIfDroppedItemNull = true;
        
        
        public override void ResetState()
        {
            base.ResetState();
            
            pickUpTime = 0;
            isMoving = true;
        }

        public override Vector3 GetPointToMove()
        {
            if (cachedItem == null)
            {
                return base.GetPointToMove();
            }
            else
            {
                return cachedItem.transform.position;
            }
        }


        public override void StartState()
        {
            base.StartState();

            if (cachedItem == null)
            {
                currentTarget = SelectionService.LastWorldClick;
                droppedItem = SelectionService.SelectedObject.Transform.GetComponent<DroppedItemBase>();
            }
            else
            {
                droppedItem = cachedItem;
                currentTarget = cachedItem.transform.position;
                
            }


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
                var storage = Machine.AIMoveManager.GoToEmptyStorage(droppedItem.StorageItem.Item, droppedItem.StorageItem.Space);
                
                if (storage != null)
                {
                    storage.AddToStorage(droppedItem.StorageItem.Item, droppedItem.StorageItem.Space, false);
                    Machine.AddExp(1);
                    WorldPopupService.StaticSpawnPopup(droppedItem.transform.position, droppedItem.StorageItem.Item, 1);
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
            cachedItem = null;
            ToIdleAnimation();
        }

        public void SetCachedItem(DroppedItemBase droppedItem)
        {
            cachedItem = droppedItem;
        }
    }
}
