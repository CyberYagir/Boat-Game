using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using PathCreation;
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
        

        public Transform Player => player;

        [Inject]
        private void Construct(MapSpawnerService mapSpawnerService, SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
            this.mapSpawnerService = mapSpawnerService;
            Player.transform.position = mapSpawnerService.Path.GetPointAtTime((float)mapSpawnerService.StartTime + ((TimeService.PlayedTime + saveDataObject.Global.TotalSecondsInGame)/divider), EndOfPathInstruction.Loop);
            
            Player.GetComponentInChildren<TrailRenderer>().Clear();
        }
        void Update()
        {
            Player.transform.position = mapSpawnerService.Path.GetPointAtTime((float)mapSpawnerService.StartTime + ((TimeService.PlayedTime + saveDataObject.Global.TotalSecondsInGame)/divider), EndOfPathInstruction.Loop);
        }
    }
}