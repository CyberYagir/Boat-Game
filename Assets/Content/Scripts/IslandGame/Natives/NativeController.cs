using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.Mobs.Mob;
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
            private Transform transform;
            private Bounds bounds;

            public INavAgentProvider NavMeshAgent => agent;

            public void Init(Transform transform)
            {
                this.transform = transform;
                agent = transform.GetComponent<INavAgentProvider>();
            }

            public void SetBounds(Bounds bounds)
            {
                this.bounds = bounds;
            }

            public Vector3 WalkToAnyPoint() => bounds.GetRandomPoint();
        }

        private NavigationManager navigationManager = new NavigationManager();
        private CharacterGrounder characterGrounder = new CharacterGrounder();
        [SerializeField] private ENativeType type;
        private INavMeshProvider navMeshProvider;

        public ENativeType NativeType => type;

        public NavigationManager AIManager => navigationManager;

        [Inject]
        private void Construct(INavMeshProvider navMeshProvider)
        {
            this.navMeshProvider = navMeshProvider;
            navMeshProvider.OnNavMeshBuild += OnNavMeshBuilded;
            print("Construct");
        }

        private void OnNavMeshBuilded()
        {
            print("Event");
            navMeshProvider.OnNavMeshBuild -= OnNavMeshBuilded;

            StartCoroutine(FrameSkip());
            
            IEnumerator FrameSkip()
            {
                yield return null;
                var point = AIManager.WalkToAnyPoint();
                if (AIManager.NavMeshAgent.TryBuildPath(point, out Vector3 newPos))
                {
                    print("move to point");
                    transform.position = newPos;
                }
            }
        }

        public override void Init(BotSpawner botSpawner)
        {
            characterGrounder.Init(transform);
            navigationManager.Init(transform);
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
