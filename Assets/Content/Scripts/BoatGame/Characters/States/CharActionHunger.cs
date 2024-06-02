using System;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionHunger : CharActionBase
    {
        public enum EHungerState
        {
            MoveToStorage,
            Eating
        }
        
        [SerializeField] private EResourceTypes type;
        [SerializeField] private Range eatTime;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private bool moveToStorage = true;
        
        
        private EHungerState state;
        private float timer;
        private RaftStorage targetStorage;
        private float eatingTime;
        private GameObject spawnedItem;

        public EHungerState CurrentState => state;

        public override void ResetState()
        {
            timer = 0;
            state = EHungerState.MoveToStorage;
        }

        public override void StartState()
        {
            base.StartState();

            targetStorage = Machine.AIMoveManager.FindResource(type);

            eatingTime = eatTime.RandomWithin();

            if (targetStorage == null)
            {
                EndState();
                return;
            }

            if (moveToStorage)
            {
                Machine.AIMoveManager.NavMeshAgent.SetStopped(false);
                MoveToPoint(targetStorage.transform.position);
            }
            else
            {
                StartEat();
            }
        }

        public override void ProcessState()
        {
            switch (CurrentState)
            {
                case EHungerState.MoveToStorage:
                    MoveToStorageLogic();
                    break;
                case EHungerState.Eating:
                    EatingLogic();
                    break;
            }
           
        }

        private void MoveToStorageLogic()
        {
            if (Machine.AIMoveManager.NavMeshAgent.IsArrived())
            {
                StartEat();
            }
        }

        private void StartEat()
        {
            Machine.AIMoveManager.NavMeshAgent.SetStopped(true);
            Animation();

            spawnedItem = Instantiate(itemPrefab, Machine.AnimationManager.RightHand);

            state = EHungerState.Eating;
        }

        private void EatingLogic()
        {
            timer += Time.deltaTime;

            if (timer >= eatingTime)
            {
                var removedItem = targetStorage.RemoveFromStorage(type);
                if (removedItem != null)
                {
                    Machine.NeedManager.AddParametersByItemName(removedItem);
                }

                EndState();
            }
        }

        public override void EndState()
        {
            base.EndState();
            
            Machine.AnimationManager.TriggerIdle();
            if (spawnedItem != null)
            {
                Destroy(spawnedItem.gameObject);
            }
        }


        public virtual void Animation() => Machine.AnimationManager.TriggerEatAnimation();

        public override bool IsCanCancel()
        {
            return CurrentState is CharActionHunger.EHungerState.MoveToStorage;
        }
    }
}
