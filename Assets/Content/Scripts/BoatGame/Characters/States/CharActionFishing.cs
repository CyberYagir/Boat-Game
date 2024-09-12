using Content.Scripts.BoatGame.Characters.Items;
using Content.Scripts.BoatGame.Ropes;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using Content.Scripts.SkillsSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionFishing : CharActionBase
    {
        
        public enum EState
        {
            MovingToPoint,
            OpenFishingRod,
            RopeSpawned,
            RopeBack,
            FishToStorage
        }
        
        [SerializeField] private InHandItem fishingRodPrefab;
        [SerializeField] private GameObject fishPrefab;
        [SerializeField] private RopeGenerator ropeGeneratorPrefab;
        [SerializeField] private Range waitTimeRange;
        [SerializeField] private ParticleSystem poofParticle;
        [SerializeField] private SkillObject fishingSkill;
        [SerializeField] private ItemObject fishItem;
        public EState CurrentState => state;


        private InHandItem spawnedRod;
        private RopeGenerator spawnedGenerator;
        private Transform spawnedFish;

        private Vector3 fishingTarget;

        private Tween tween;
        
        [SerializeField, ReadOnly] private EState state;
        [SerializeField, ReadOnly] private float waitTime;
        [SerializeField, ReadOnly] private float timer;

        public override void ResetState()
        {
            base.ResetState();
            state = EState.MovingToPoint;
            stuckTimer = 0;
            Machine.AnimationManager.TriggerFishingAnimation(true);
            timer = 0;
            tween = null;
        }

        public override void StartState()
        {
            base.StartState();

            fishingTarget = Machine.SelectionService.LastWorldClick;
            waitTime = waitTimeRange.RandomWithin() * Machine.Character.GetSkillMultiply(fishingSkill.SkillID);

            Agent.SetStopped(false);
            MoveToPoint(fishingTarget);
        }


        public override void ProcessState()
        {
            base.ProcessState();


            switch (CurrentState)
            {
                case EState.MovingToPoint:
                    MovingToPointLogic();
                    break;
                case EState.OpenFishingRod:
                    OpenFishingRodLogic();
                    break;
                case EState.RopeSpawned:
                    RopeSpawnedLogic();
                    break;
                case EState.RopeBack:
                    break;
                case EState.FishToStorage:
                    FishToStorageLogic();
                    break;
            }
        }

        protected override void OnMoveEnded()
        {
            SpawnItemInHands();
            state = EState.OpenFishingRod;
        }

        private void FishToStorageLogic()
        {
            if (Machine.AIMoveManager.NavMeshAgent.IsArrived())
            {
                var storage = Machine.AIMoveManager.GoToEmptyStorage(fishItem, 1);
                if (storage == null)
                {
                    DropFish();
                    WorldPopupService.StaticSpawnCantPopup(Machine.transform.position);
                }
                else
                {
                    storage.AddToStorage(fishItem, 1);
                    Machine.AnimationManager.TriggerHoldFishAnimation(false);
                    Destroy(spawnedFish.gameObject);
                    Machine.AddExp(1);
                }

                EndState();
            }
        }

        private void RopeSpawnedLogic()
        {
            timer += TimeService.DeltaTime;

            if (timer >= waitTime)
            {
                if (spawnedFish == null)
                {
                    spawnedFish = Instantiate(fishPrefab, new Vector3(fishingTarget.x, -10, fishingTarget.z), Quaternion.identity).transform;

                    spawnedFish.parent = spawnedGenerator.Point;

                    spawnedFish.DOLocalMoveY(-0.7f, 2f).onComplete += delegate
                    {
                        spawnedGenerator.OnRopeEnded += OnRopeEnded;
                        spawnedGenerator.RopeBack();
                        state = EState.RopeBack;
                    };
                }
            }
        }

        private void OpenFishingRodLogic()
        {
            timer += TimeService.DeltaTime;

            if (timer >= 2.2f)
            {
                spawnedGenerator = Instantiate(ropeGeneratorPrefab, spawnedRod.EndPoint.position, Quaternion.identity, null);
                spawnedGenerator.gameObject.SetActive(false);
                if (spawnedGenerator != null)
                {
                    timer = 0;
                    spawnedGenerator.transform.position = spawnedRod.EndPoint.position;
                    spawnedGenerator.gameObject.SetActive(true);
                    spawnedGenerator.SetTargetPosition(fishingTarget);
                    spawnedGenerator.GenerateRope();
                    spawnedGenerator.SetFirstChainPointPos(spawnedRod.EndPoint);
                    state = EState.RopeSpawned;
                }
            }
        }



        private void OnRopeEnded()
        {
            spawnedFish.transform.parent = Machine.AnimationManager.FishPoint;
            spawnedFish.localPosition = Vector3.zero;
            spawnedFish.localEulerAngles = Vector3.zero;
            spawnedFish.localScale = Vector3.one;

            spawnedFish.ChangeLayerWithChilds(LayerMask.NameToLayer("Raft"));
            
            Machine.AnimationManager.TriggerFishingAnimation(false);
            Machine.AnimationManager.TriggerHoldFishAnimation(true);
            Machine.AnimationManager.TriggerIdle();
            
            state = EState.FishToStorage;
            DespawnItems();
            
            
            FindEmptyStorage();
        }

        private void FindEmptyStorage()
        {
            var storage = Machine.AIMoveManager.GoToEmptyStorage(fishItem, 1, false);

            if (storage != null)
            {
                Machine.AIMoveManager.NavMeshAgent.SetStopped(false);
                MoveToPoint(storage.transform.position);

            }
            else
            {
                tween = DOVirtual.DelayedCall(1f, delegate
                {
                    DropFish();
                    EndState();
                });
            }
        }


        public void DropFish()
        {
            spawnedFish.transform.parent = null;
            var rb = spawnedFish.gameObject.AddComponent<Rigidbody>();
            rb.AddForce(transform.forward * Random.Range(100, 200) + Vector3.up * Random.Range(50, 150));
            rb.AddTorque(Random.insideUnitSphere * Random.Range(20, 50));
            Machine.AnimationManager.TriggerHoldFishAnimation(false);
        }

        private void DespawnItems()
        {
            poofParticle.Play();
            if (spawnedGenerator)
            {
                Destroy(spawnedGenerator.gameObject);
            }

            if (spawnedRod != null)
            {
                Destroy(spawnedRod.gameObject);
            }
        }


        public void SpawnItemInHands()
        {
            if (spawnedRod == null)
            {

                Agent.SetStopped(true);

                spawnedRod = Instantiate(fishingRodPrefab, Machine.AnimationManager.RightHand);


                fishingTarget.y = Machine.transform.position.y;

                var dir = fishingTarget - Machine.transform.position;
                Machine.transform.DORotateQuaternion(Quaternion.LookRotation(dir), 0.5f);
            }

            Machine.AnimationManager.TriggerFishingAnimation();
        }
        

        public override void EndState()
        {
            base.EndState();

            if (spawnedFish != null)
            {
                if (spawnedFish.GetComponent<Rigidbody>())
                {
                    Destroy(spawnedFish.gameObject, 5f);
                }

                spawnedFish = null;
            }

            if (spawnedGenerator != null || spawnedRod != null)
            {
                DespawnItems();
            }
            
            ToIdleAnimation();

            tween.Kill();
        }

        public override bool IsCanCancel()
        {
            return CurrentState != EState.RopeBack && CurrentState != EState.FishToStorage;
        }
    }
}
