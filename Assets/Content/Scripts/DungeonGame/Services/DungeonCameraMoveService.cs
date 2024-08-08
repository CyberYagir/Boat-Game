using System;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonCameraMoveService : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Camera camera;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Vector3 offcet;
        
        private DungeonCharactersService charactersService;

        public Transform CameraHolder => cameraHolder;

        public Camera Camera => camera;


        [Inject]
        private void Construct(DungeonCharactersService charactersService)
        {
            this.charactersService = charactersService;
        }

        private void Update()
        {
            var pos = Vector3.zero;
            foreach (var c in charactersService.SpawnedCharacters)
            {
                pos += c.transform.position;
            }
            pos /= charactersService.SpawnedCharacters.Count;

            var nextPos = pos + offcet;
            CameraHolder.position = Vector3.Lerp(CameraHolder.position, nextPos, TimeService.DeltaTime * moveSpeed);
        }
    }
}
