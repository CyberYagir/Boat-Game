using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class NativeWorldIcon : MonoBehaviour
    {
        [SerializeField] private Transform part;
        [SerializeField] private Transform point;
        private Camera mainCamera;
        private SelectionService selectionService;

        [Inject]
        private void Construct(SelectionService selectionService)
        {
            this.selectionService = selectionService;
            mainCamera = selectionService.Camera;
            
            selectionService.OnChangeSelectObject += SelectionServiceOnOnChangeSelectObject;
        }

        private void SelectionServiceOnOnChangeSelectObject(ISelectable obj)
        {
            
            if (obj == null)
            {
                gameObject.SetActive(true);
                return;
            }
            else if (obj.Transform == part)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void LateUpdate()
        {
            if (selectionService.SelectedObject == null || selectionService.SelectedObject.Transform != part)
            {
                transform.position = mainCamera.WorldToScreenPoint(point.transform.position);
            }
        }
    }
}
