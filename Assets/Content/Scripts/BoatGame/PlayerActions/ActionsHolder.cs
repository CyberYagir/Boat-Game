using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class ActionsHolder : MonoBehaviour, ISelectable
    {
        public List<PlayerAction> PlayerActions => playerActions;
        public Transform Transform => transform;
        public Transform TransformOrCustomTransform => customTransformPoint != null ? customTransformPoint : Transform;
        public ISelectable Transfered => null;

        private Transform customTransformPoint;
        
        [SerializeField] private List<PlayerAction> playerActions = new List<PlayerAction>();
        private SelectionService selectionService;


        [Inject]
        public void Construct(SelectionService selectionService, GameDataObject gameDataObject)
        {
            this.selectionService = selectionService;
            foreach (var ac in playerActions)
            {
                ac.Init(selectionService, gameDataObject);
            }
        }

        private void OnDestroy()
        {
            DisableSelection();
        }

        public void DisableSelection()
        {
            if (selectionService == null) return;
            if (selectionService.SelectedObject == null) return;
            if ((ActionsHolder) selectionService.SelectedObject == this)
            {
                selectionService.ClearSelectedObject();
                customTransformPoint = null;
            }
        }

        public void RegisterCustomTransformPoint(Transform transferedTransform)
        {
            customTransformPoint = transferedTransform;
        }
    }
}
