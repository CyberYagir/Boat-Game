using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.Mobs.Mob;
using Pathfinding;
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
        Cook
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
                this.transform = transform;
                agent = transform.GetComponent<INavAgentProvider>();
            }

            public void SetBounds(Bounds bounds)
            {
                this.bounds = bounds;
            }

            public Vector3 WalkToAnyPoint() => bounds.GetRandomPoint();

            public bool IsAvailablePoint(Vector3 pos)
            {
                return navMesh.IsAvailablePoint(pos);
            }
        }

        private VillageDataCollector villageData;
        private NavigationManager navigationManager = new NavigationManager();
        private CharacterGrounder characterGrounder = new CharacterGrounder();
        [SerializeField] private ENativeType type;
        private INavMeshProvider navMeshProvider;

        public ENativeType NativeType => type;

        public NavigationManager AIManager => navigationManager;

        public VillageDataCollector VillageData => villageData;

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
                yield return null;
                GetComponent<AIPath>().enabled = true;
                var point = AIManager.WalkToAnyPoint();
                if (AIManager.NavMeshAgent.TryBuildPath(point, out Vector3 newPos))
                {
                    transform.position = newPos;
                }
            }
        }

        public override void Init(BotSpawner botSpawner)
        {
            characterGrounder.Init(transform);
            navigationManager.Init(transform, navMeshProvider);
            villageData = GetComponentInParent<VillageDataCollector>();
            base.Init(botSpawner);
            
        }

        public void SetVillageBounds(Bounds bounds)
        {
            navigationManager.SetBounds(bounds);
        }

        public void OnUpdate()
        {
            characterGrounder.PlaceUnitOnGround();
        }
    }
}
