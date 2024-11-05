using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class NativeWorldIcon : MonoBehaviour
    {
        [SerializeField] private Transform part;
        [SerializeField] private Transform point;
        [SerializeField] private GraphicRaycaster canvas;
        private Transform lerpPoint;
        private Camera mainCamera;
        private SelectionService selectionService;

        [Inject]
        private void Construct(SelectionService selectionService)
        {
            this.selectionService = selectionService;
            mainCamera = selectionService.Camera;
            
            selectionService.AddRaycaster(canvas);

            lerpPoint = new GameObject("LerpPoint").transform;

            lerpPoint.transform.position = point.transform.position;
        }
        

        private void LateUpdate()
        {
            lerpPoint.transform.position = Vector3.Lerp(lerpPoint.transform.position, point.transform.position, 10f * Time.deltaTime);
            transform.position = mainCamera.WorldToScreenPoint(lerpPoint.transform.position);
            
            
            if (selectionService.SelectedCharacter != null && selectionService.SelectedCharacter.CurrentState == EStateType.VillageViewInfo)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 5f);
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 5);
            }
        }

        public void ActiveAction()
        {
            var action = GetComponentInParent<ISelectable>();
            selectionService.SelectObject(action, false);
            action.PlayerActions[0].Action();
            selectionService.ClearSelectedObject();
        }
    }
}
