using System;
using Content.Scripts.BoatGame.Ropes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionHooking : CharActionBase
    {
        public enum States
        {
            MoveToPoint,
            HookSpawned,
            HookBack,
            MoveToStorage
        }
        
        [SerializeField] private RopeGenerator ropeGeneratorPrefab;
        [SerializeField] private ParticleSystem poofParticle;
        private RopeGenerator spawnedGenerator;
        
        private Transform selectedItem;
        private WaterItem.DropData resourceData;

        [SerializeField, ReadOnly] private States state;
        public States State => state;

        public override void ResetState()
        {
            base.ResetState();
            state = States.MoveToPoint;
            selectedItem = null;
            resourceData = null;
            Agent.SetStopped(false);
        }


        public override void StartState()
        {
            base.StartState();
            
            selectedItem = Machine.SelectionService.SelectedObject.Transform;
            resourceData = selectedItem.GetComponent<WaterItem>().Drop;
            MoveToPoint(selectedItem.position);
        }

        protected override void OnMoveEnded()
        {
            state = States.HookSpawned;
            
            Machine.AnimationManager.TriggerHook();
            
            spawnedGenerator = Instantiate(ropeGeneratorPrefab, Machine.AnimationManager.RightHand.position, Quaternion.identity, null);
            spawnedGenerator.gameObject.SetActive(false);
            if (spawnedGenerator != null)
            {
                spawnedGenerator.transform.position = Machine.AnimationManager.RightHand.position;
                spawnedGenerator.gameObject.SetActive(true);
                spawnedGenerator.SetTargetPosition(selectedItem.position + selectedItem.GetComponent<Rigidbody>().velocity);
                spawnedGenerator.GenerateRope();
                spawnedGenerator.SetFirstChainPointPos(Machine.AnimationManager.RightHand);
                
                spawnedGenerator.OnRopeAnimationEnded += OnRopeAnimationEnded;
            }
        }

        private void OnRopeAnimationEnded()
        {
            selectedItem.GetComponent<WaterItem>().DisableItem();
            selectedItem.parent = spawnedGenerator.Point;
            state = States.HookBack;
            spawnedGenerator.RopeBack();
            spawnedGenerator.OnRopeEnded += OnRopeEnded;
        }

        private void OnRopeEnded()
        {
            Machine.AnimationManager.TriggerIdle();
            Machine.AnimationManager.TriggerHoldFishAnimation(true);

            if (selectedItem == null || selectedItem.GetComponent<WaterItem>().IsOnDeath)
            {
                EndState();
                return;
            }
            
            selectedItem.parent = Machine.AnimationManager.FishPoint;
            selectedItem.ChangeLayerWithChilds(LayerMask.NameToLayer("Raft"));
            
            selectedItem.localScale = Vector3.one;
            selectedItem.localPosition = Vector3.zero;
            selectedItem.localEulerAngles = Vector3.zero;
            
            
            
            DestoryRope();

            FindStorage();
        }

        private void DestoryRope()
        {
            if (spawnedGenerator != null)
            {
                poofParticle.Play(true);
                Destroy(spawnedGenerator.gameObject);
            }
        }

        private void FindStorage()
        {
            Machine.AIMoveManager.NavMeshAgent.SetStopped(false);
            var storage = Machine.AIMoveManager.GoToEmptyStorage(resourceData.Item, resourceData.Value);
            if (storage == null)
            {
                DropItem();
                EndState();
            }
            else
            {
                Machine.AIMoveManager.NavMeshAgent.SetDestination(storage.transform.position);
                state = States.MoveToStorage;
            }
        }

        public override void ProcessState()
        {
            base.ProcessState();

            switch (State)
            {
                case States.MoveToPoint:
                    MovingToPointLogic();
                    break;
                case States.HookSpawned:
                    ItemCheckLogic();
                    break;
                case States.HookBack:
                    ItemCheckLogic();
                    break;
                case States.MoveToStorage:
                    ItemToStorageLogic();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public void ItemCheckLogic()
        {
            if (selectedItem == null || selectedItem.GetComponent<WaterItem>().IsOnDeath)
            {
                EndState();
                return;
            }
        }
        
        private void ItemToStorageLogic()
        {
            if (Machine.AIMoveManager.NavMeshAgent.IsArrived())
            {
                var storage = Machine.AIMoveManager.GoToEmptyStorage(resourceData.Item, resourceData.Value);
                if (storage == null)
                {
                    DropItem();
                }
                else
                {
                    storage.AddToStorage(resourceData.Item, resourceData.Value);
                    Machine.AnimationManager.TriggerHoldFishAnimation(false);
                    Machine.AddExp(1);
                    Destroy(selectedItem.gameObject);
                }
                
                EndState();
            }
        }
        
        public void DropItem()
        {
            if (selectedItem != null)
            {
                selectedItem.transform.parent = null;
                var rb = selectedItem.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(transform.forward * Random.Range(100, 200) + Vector3.up * Random.Range(50, 150));
                rb.AddTorque(Random.insideUnitSphere * Random.Range(20, 50));
            }

            Machine.AnimationManager.TriggerHoldFishAnimation(false);
        }

        public override void EndState()
        {
            base.EndState();
            DestoryRope();

            if (state != States.HookBack || state != States.HookSpawned)
            {
                if (selectedItem != null)
                {
                    selectedItem.transform.parent = null;
                    var item = selectedItem.GetComponent<WaterItem>();
                    if (item.IsStopped)
                    {
                        item.EnableItem();
                    }
                }
            }

            ToIdleAnimation();
        }
    }
}
