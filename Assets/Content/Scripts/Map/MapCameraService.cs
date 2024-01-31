using System;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapCameraService : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private Vector3 offcet;
        private MapMoverService mapMoverService;

        [Inject]
        private void Construct(MapMoverService mapMoverService)
        {
            this.mapMoverService = mapMoverService;
        }

        private void Update()
        {
            camera.transform.position = mapMoverService.Player.transform.position + offcet;
        }
    }
}
