using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Sources;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{

    public class CharActionGetFromSource : CharActionMoveTo
    {

        private float pickUpTime;
        private bool isMoving;
        protected Vector3 currentTarget;
        private ISourceObject targetSource;


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

        public override void ProcessState()
        {
            base.ProcessState();

            if (!isMoving)
            {
                pickUpTime += TimeService.DeltaTime;

                if (targetSource == null)
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
            ItemObject item = targetSource.GetFromItem();
            if (item)
            {
                var storage = Machine.AIMoveManager.GoToEmptyStorage(1);

                if (storage != null)
                {
                    storage.AddToStorage(item, 1, false);
                    Machine.AddExp(1);
                    WorldPopupService.StaticSpawnPopup(currentTarget, item, 1);
                }
                else
                {

                    WorldPopupService.StaticSpawnCantPopup(currentTarget);
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
