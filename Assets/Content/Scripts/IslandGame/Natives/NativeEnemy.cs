using System;
using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.Mobs.MobSnake;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.IslandGame.Natives
{
    public class NativeEnemy : SpawnedMobAggressive
    {
        [SerializeField, FoldoutGroup("Data")] private PlayerCharacter.RagdollController ragdollController;
        [SerializeField, ReadOnly] private string uid;
        private NativeController.NavigationManager navigationManager = new NativeController.NavigationManager();
        private CharacterGrounder characterGrounder = new CharacterGrounder();
        private INavMeshProvider navMeshProvider;
        private SaveDataObject saveDataObject;

        public NativeController.NavigationManager AIManager => navigationManager;
        
        [Inject]
        private void Construct(INavMeshProvider navMeshProvider, SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
            this.navMeshProvider = navMeshProvider;
            navMeshProvider.OnNavMeshBuild += OnNavMeshBuilded;
        }

        public override void Init(BotSpawner botSpawner, bool initStateMachine = true)
        {
            navigationManager.Init(transform, navMeshProvider);
            characterGrounder.Init(transform);
            navigationManager.SetBounds(new Bounds(transform.position, Vector3.one * 10f));
            base.Init(botSpawner, initStateMachine);
        }
        
        public void InitPillager(Random rnd)
        {
            uid = Extensions.GenerateSeededGuid(rnd).ToString();

            if (saveDataObject.GetTargetIsland().IsPillagerDead(uid))
            {
                SetIsMomentalDeath(true);
                Damage(1000, gameObject);
            }
        }

        public override void OnOnAttackedStart()
        {
            base.OnOnAttackedStart();
            Animations.StopMove();
            navigationManager.NavMeshAgent.SetStopped(true);
        }

        public override void OnOnAttackedEnd()
        {
            base.OnOnAttackedEnd();
            navigationManager.NavMeshAgent.SetStopped(false);
            if (!navigationManager.NavMeshAgent.IsArrived())
            {
                Animations.StartMove();
            }
        }

        public void Update()
        {
            characterGrounder.PlaceUnitOnGround();
        }

        public override void Death()
        {
            base.Death();
            navMeshProvider.OnNavMeshBuild -= OnNavMeshBuilded;
            AIManager.NavMeshAgent.Disable();
            ragdollController.ActiveRagdoll();
            saveDataObject.GetTargetIsland().AddDeadPillager(uid);
        }

        private void OnNavMeshBuilded()
        {
            navMeshProvider.OnNavMeshBuild -= OnNavMeshBuilded;
            StartCoroutine(FrameSkip());

            IEnumerator FrameSkip()
            {
                while (!navMeshProvider.GetNavMeshByID(0).isScanned) // wait for async scan if this in process
                {
                    yield return null;
                }

                if (!IsDead)
                {
                    GetComponent<AIPath>().enabled = true;
                    StateMachine.Init(this);
                }
            }
        }


    }
}
