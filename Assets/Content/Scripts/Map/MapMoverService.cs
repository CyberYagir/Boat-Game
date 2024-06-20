using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapMoverService : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float divider = 5000f;
        private MapSpawnerService mapSpawnerService;
        private SaveDataObject saveDataObject;
        private TrailRenderer trail;


        public Transform Player => player;

        [Inject]
        private void Construct(MapSpawnerService mapSpawnerService, SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
            this.mapSpawnerService = mapSpawnerService;
            MovePlayer();
            trail = Player.GetComponentInChildren<TrailRenderer>();
            trail.Clear();
        }

        private void MovePlayer()
        {
            Player.transform.position = mapSpawnerService.Path.GetPointAtTime((float) mapSpawnerService.StartTime + ((saveDataObject.Global.TotalSecondsOnRaft + TimeService.PlayedBoatTime) / divider), EndOfPathInstruction.Loop);
        }

        void Update()
        {
            MovePlayer();
        }

        public void GoToIsland(int seed)
        {
            var timesDelta = GetTimeDistance(seed);

            TimeService.AddPlayedBoatTime(timesDelta * divider);

            MovePlayer();
            
            trail.Clear();
            
        }

        public float GetTimeDistance(int seed)
        {
            var island = mapSpawnerService.GetIslandBySeed(seed);
            var islandTime = mapSpawnerService.Path.GetClosestTimeOnPath(island.transform.position);
            var playerTime = mapSpawnerService.Path.GetClosestTimeOnPath(Player.transform.position);

            var timesDelta = islandTime - playerTime;

            if (timesDelta < 0)
            {
                timesDelta = 1 + timesDelta;
            }

            return timesDelta;
        }
    }
}
