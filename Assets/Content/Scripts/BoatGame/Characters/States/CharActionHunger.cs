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

            Machine.AIMoveManager.NavMeshAgent.isStopped = false;
            Machine.AIMoveManager.NavMeshAgent.SetDestination(targetStorage.transform.position);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
        }

        private void MoveToStorageLogic()
        {
            if (Machine.AIMoveManager.NavMeshAgent.IsArrived())
            {
                Machine.AIMoveManager.NavMeshAgent.isStopped = true;
                Animation();

                spawnedItem = Instantiate(itemPrefab, Machine.AnimationManager.RightHand);
                
                state = EHungerState.Eating;
            }
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
    }
}
