using System;
using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.Mobs.Mob;
using Content.Scripts.Mobs.MobCrab;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame.Natives
{
    public enum ENativeType
    {
        Man,
        Female,
        Shaman,
        Blacksmith,
        Cook,
        Mage,
        ShamanMedival,
        Fabricator
    }


    public class NativeController : SpawnedMob
    {
        [System.Serializable]
        public class NavigationManager
        {
            private INavAgentProvider agent;
            private INavMeshProvider navMesh;
            private Transform transform;
            private Bounds bounds;

            public INavAgentProvider NavMeshAgent => agent;

            public void Init(Transform transform, INavMeshProvider navMeshProvider)
            {
                navMesh = navMeshProvider;
                this.transform = transform;
                agent = transform.GetComponent<INavAgentProvider>();
            }

            public void SetBounds(Bounds bounds)
            {
                this.bounds = bounds;
            }

            public Vector3 WalkToAnyPoint()
            {
                var point = bounds.GetRandomPoint();
                return new Vector3(point.x, agent.Transform.position.y, point.z);
            }

            public bool IsAvailablePoint(Vector3 pos)
            {
                return navMesh.IsAvailablePoint(pos);
            }

            public void MoveToPoint(Vector3 attackedTransformPosition)
            {
                
            }
        }
        [SerializeField] private ENativeType type;
        [SerializeField, ReadOnly] private string skinUid;

        private VillageDataCollector villageData;
        private NavigationManager navigationManager = new NavigationManager();
        private CharacterGrounder characterGrounder = new CharacterGrounder();
        private INavMeshProvider navMeshProvider;

        public ENativeType NativeType => type;

        public NavigationManager AIManager => navigationManager;

        public VillageDataCollector VillageData => villageData;

        public string SkinUid => skinUid;

        [Inject]
        private void Construct(INavMeshProvider navMeshProvider)
        {
            this.navMeshProvider = navMeshProvider;
            navMeshProvider.OnNavMeshBuild += OnNavMeshBuilded;
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
                GetComponent<AIPath>().enabled = true;
                StateMachine.Init(this);
            }
        }

        public override void OnOnAttackedStart()
        {
            base.OnOnAttackedStart();
            ChangeStateTo(EMobsState.Stop);
        }

        public override void OnOnAttackedEnd()
        {
            base.OnOnAttackedEnd();
            ChangeStateTo(EMobsState.Idle);
        }


        public override void Init(BotSpawner botSpawner, bool initStateMachine = true)
        {
            characterGrounder.Init(transform);
            navigationManager.Init(transform, navMeshProvider);
            villageData = GetComponentInParent<VillageDataCollector>();
            
            navigationManager.SetBounds(villageData.Bounds());
            base.Init(botSpawner, false);
            
        }

        public override void Damage(float dmg, GameObject sender)
        {
            dmg = 0;
            base.Damage(dmg, sender);
        }

        public void OnUpdate()
        {
            characterGrounder.PlaceUnitOnGround();
        }

        [Button]
        public void GenerateID() => skinUid = Guid.NewGuid().ToString();
    }
}
