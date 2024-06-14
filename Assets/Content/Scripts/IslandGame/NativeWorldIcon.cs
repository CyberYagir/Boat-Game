using System;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class NativeWorldIcon : MonoBehaviour
    {
        private Camera mainCamera;
        [Inject]
        private void Construct(SelectionService selectionService)
        {
            mainCamera = selectionService.Camera;
        }

        private void LateUpdate()
        {
            transform.LookAt(mainCamera.transform.position);
        }
    }
}
